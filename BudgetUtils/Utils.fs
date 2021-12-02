namespace BudgetManager.Utils

open System.Text.RegularExpressions
open System

module NodeCodeUtilities =

    let Parent ( code : string ) =

        let hyphensMatches = Regex.Matches(code, "\-" ) 

        let lastHyphenMatchIndex = hyphensMatches.[hyphensMatches.Count-1].Index

        let parent = code.Substring(0,lastHyphenMatchIndex)

        parent

    let Ancestors ( code : string ) = 
        
        let ancestors =   code 
                          |> Seq.unfold ( fun fc -> if( fc = "" )
                                                    then None
                                                    else Some( fc, Parent fc ) )
                          |> Seq.toArray
    
        ancestors

    let ParentAndAncestors ( code : string ) =

        let parent = Parent code

        let ancestors = Ancestors code

        parent, ancestors
    ///
    ///Elimina un nivel (el más alto del código de una cuenta)
    let RemoveOneLevel( accountCode : string ) = 

        let hyphens = Regex.Matches(accountCode, "\\-")
    
        let remainingAccount = match hyphens.Count >= 2 with
                               | true -> accountCode.Remove(0, hyphens.[1].Index)
                               | false -> ""
    
        remainingAccount 

module DataLoadFunctions =   

    let MappedParentAndAncestors ( accountsMapArray :  seq<(string * string)>) =

        let accountsMap = accountsMapArray
                          |> Map.ofSeq

        let mappedParentAndAncestors = 
            accountsMap
            |> Map.toSeq
            |> Seq.map ( fun (fc, fl) -> fc, fl, NodeCodeUtilities.ParentAndAncestors fc)
            |> Seq.map ( fun ( fc, fl, (fp, fsa)) -> fc, fl, fp,  fsa
                                                                  |> Seq.map ( fun fac -> if( not (accountsMap.ContainsKey(fac))) then sprintf "Se esta intentando procesar la cuenta %s, 
                                                                                                                                                pero en el archivo no aparece cuenta padre, verifique su archivo" fac |> failwith 
                                                                                          fac, accountsMap.Item fac )
                                                                  |> Seq.toArray )
            |> Seq.toArray

        mappedParentAndAncestors

    ///
    ///Devuelve el último nivel de un código; es decir, la hoja de la rama.
    let Leaf ( code : string ) =

        let hyphens = Regex.Matches( code, "\\-")

        let leaf = match hyphens.Count with
                   | 0 -> "-"+code
                   | _ ->code.Remove(0,hyphens.[hyphens.Count-1].Index)

        leaf

    let OptionChildrenCodes ( dict : (string * string)[] ) ( code : string ) =
 
        let childrenCodes = dict 
                            |> Seq.filter ( fun (fc,fl) -> Regex.Match(fc, "^"+code+"\\-[0-9]{2}$").Success )
                            |> Seq.toArray

        let optionChildrenCodes = match childrenCodes.Length with
                                  | 0 -> None
                                  | _ -> Some(childrenCodes)
              
        optionChildrenCodes

    ///
    ///Recibe: array con: - names
    ///                   - labels
    ///        label con toda la jerarquía (ejemplo, -a-ab-abc)
    ///Devuelve el name que correspnde al label
    ///Se usa cuando los labels en el árbol no contienen la jerarquía ( ejemplo: con jerarquia (-01, -a), (-01-02, -a-ab), sin jerarquía (-01, -a), (-01-02, -ab)
    ///NOTA: cuando es jerárquico no se necesita este método, porque en ese caso simplemente se busca en el árbol
    let MatchingCode ( dict : (string * string)[] ) ( label : string ) =

        let ancestors = NodeCodeUtilities.Ancestors label

        let matchingCode = match ancestors.Length with
                           | 0 -> "###"
                           | _ ->  let ancestorsOrder =  ancestors.Length - 1
                                                          |> Seq.unfold ( fun fi -> if ( fi < 0 )
                                                                                    then None
                                                                                    else Some ( fi , ( fi - 1 ) ) )
                                                          |> Seq.toArray
              
                                   let optionRootCode = dict
                                                         |> Seq.tryFind ( fun ( fc, fl ) -> fl = ancestors.[ancestorsOrder.[0]] )

                                   match optionRootCode.IsSome with
                                   | true -> let ( rootCode, _ ) = optionRootCode.Value
                                             ancestorsOrder
                                             |> Seq.skip 1
                                             |> Seq.fold ( fun acc fo -> let (childCode, _) = let optionChildrenCodes = OptionChildrenCodes dict acc
                                                                                              let optionMatchingCode = match optionChildrenCodes.IsSome with
                                                                                                                       | true -> optionChildrenCodes.Value
                                                                                                                                 |> Seq.tryFind ( fun ( fcc, fcl ) -> fcl = ancestors.[fo] )
                                                                                                                       | false -> None
                                                                                              match optionMatchingCode.IsSome with
                                                                                              | true -> optionMatchingCode.Value
                                                                                              | false -> "###", "###" 
                                                                                      
                                                                         childCode ) rootCode
                                   | false -> "###"

        let found = false = Regex.Match( matchingCode, "\\#\\#\\#").Success

        found , matchingCode

module CalculatorUtilities = 
    
    let convertStringToInt ( i : string ) =

        let ( success , value ) = Int32.TryParse(i)
                
        match success with
        | true -> value
        | false -> -1

    let ExtractLagAccountAndLabel ( content : string ) =

        let sign = match content.Substring(0,1) with
                   | ">" -> 1
                   | "<" -> -1
                   | _ -> 0
               
        let stringLag = Regex.Match(content, "[0-9]{1,2}")

        let lag = match stringLag.Success with
                  | true -> match stringLag.Index with
                            | 1 -> sign * convertStringToInt (content.Substring(stringLag.Index,stringLag.Length) )
                            | _ -> 0
                  | false -> 0
              
        let slash = Regex.Match(content, "\\/")

        let label = match slash.Success with
                    | true -> content.Remove(0,slash.Index+1)
                    | false -> ""

        let account = match label <> "" && lag <> 0 with
                      | true -> let tailContent = content.Remove(0, stringLag.Index+stringLag.Length)
                                tailContent.Substring(0,tailContent.Length-label.Length-1) 
                      | false -> ""

        lag, account, label

    ///
    ///Recibe un string de la forma -99(lag) o vacío, código de cuenta
    ///Devuelve lag y código de cuenta
    let ExtractLagAndAccountCode ( content : string ) =

        let sign = match content.Substring(0,1) with
                   | "-" -> -1
                   | _ -> 1

        let stringLag = Regex.Match(content, "[0-9]{1,2}")

        let ( lag, accountCodeIndex )  =  match stringLag.Success with
                                          | true -> match sign with
                                                    | 1 -> match stringLag.Index with 
                                                           | 0 ->  convertStringToInt (content.Substring(0,stringLag.Length) ), stringLag.Length 
                                                           | _ -> 0, 0
                                                    | -1 -> match stringLag.Index with 
                                                            | 1 -> ( sign * convertStringToInt (content.Substring(1,stringLag.Length) ) ), stringLag.Length + 1
                                                            | _ -> 0, 0 
                                                    | _ -> 0, 0
                                          | false -> 0, 0

        let accountCode = content.Remove(0, accountCodeIndex)

        lag, accountCode

module RecordConversion =
    //Cargue StructuralTree
    let CreateLiteStructuralTreeRecord ( outReg : string) ( structuralTreeName : string ) =

        let outRegSplit = outReg.Split([|'\t'|] )

        match outRegSplit.[0] = structuralTreeName with
        | true -> "Util", outRegSplit
        | false -> "Inutil", outRegSplit