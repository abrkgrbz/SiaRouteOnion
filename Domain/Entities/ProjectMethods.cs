﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities
{
    public class ProjectMethods:BaseEntity
    {
        public string MethodName { get; set; }
        public int Size { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
