using DevExpress.ExpressApp;
using System;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace BudgetManager.Module
{
    public static class AppParameters
    {

        private static RepositoryEngine? repositoryEngine;

        public static RepositoryEngine RepositoryEngine
        {
            get
            {
                if (repositoryEngine != null) return repositoryEngine.Value;
                if (!ConfigurationManager.AppSettings.AllKeys.Contains("RepositoryEngine")) throw new UserFriendlyException("Error en el archivo de configuración de la aplicación. Parámetro -RepositoryEngine- ausente. Contacte al administrador");
                if (!Enum.TryParse(ConfigurationManager.AppSettings["RepositoryEngine"], out RepositoryEngine parameter)) throw new UserFriendlyException("Error en el archivo de configuración de la aplicación. El valor del parámetro -RepositoryEngine- es invalido. Contacte al administrador");
                if (!Enum.IsDefined(typeof(RepositoryEngine), parameter)) throw new UserFriendlyException("Error en el archivo de configuración de la aplicación. El valor del parámetro -RepositoryEngine- es invalido. Contacte al administrador");
                repositoryEngine = parameter;
                return repositoryEngine.Value;
            }
        }

        private static Fabric fabric;
        public static Fabric Fabric
        {
            get
            {
                if (!(fabric is null)) return fabric;
                fabric = new Fabric(RepositoryEngine);
                return fabric;
            }
        }

        private static CultureInfo _CulturalReference;
        public static CultureInfo CulturalReference
        {
            get
            {
                if (_CulturalReference != null) return _CulturalReference;
                if (!ConfigurationManager.AppSettings.AllKeys.Contains("CultureReference")) throw new UserFriendlyException("Error en el archivo de configuración de la aplicación. Parámetro -CultureReference- ausente. Contacte al administrador");
                try
                {
                    _CulturalReference = new CultureInfo(ConfigurationManager.AppSettings["CultureReference"]);
                    return _CulturalReference;
                }
                catch
                {
                    throw new UserFriendlyException("Error en el archivo de configuración de la aplicación. El valor del parámetro -CultureReference- es inválido. Contacte al administrador");
                }
            }
        }

    }
}