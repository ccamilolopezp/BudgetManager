using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using System.Collections.Generic;
using System.Linq;

namespace BudgetManager.Module.BusinessManagers.Infrastructure
{
    public static class GeneralUtils
    {
        public static bool IsRoleInUser(PermissionPolicyUser user, List<string> acceptableRoles)
        {
            if (acceptableRoles?.Count == 0) return false;
            if (user == null) return false;
            if (user.Roles.Count == 0) return false;
            return acceptableRoles.Intersect(user.Roles.Select(role => role.Name)).Any();
        }
    }
}
