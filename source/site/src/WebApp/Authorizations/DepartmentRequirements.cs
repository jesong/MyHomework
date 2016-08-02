namespace MyHomework.WebApp.Authorizations
{
    using Microsoft.AspNetCore.Authorization;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class DepartmentRequirements : IAuthorizationRequirement
    {
        public DepartmentRequirements(string departmentId)
        {
            DepartmentId = departmentId;
        }

        public string DepartmentId { get; set; }
    }
}
