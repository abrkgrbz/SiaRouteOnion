﻿using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Newtonsoft.Json;
using Domain.Entities;

namespace Persistence.Utils
{
    public class AuditEntry
    {
        
        public AuditEntry(EntityEntry entry)
        {
            Entry=entry; 
        }

        public EntityEntry Entry { get; }
        public string UserId { get; set; }
        public string TableName { get; set; }
        public DateTime DateTime { get; set; }
        public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
        public AuditType AuditType { get; set; }
        public List<string> ChangedColumns { get; } = new List<string>();
        public List<PropertyEntry> TemporaryProperties { get; } = new List<PropertyEntry>();

        public bool HasTemporaryProperties => TemporaryProperties.Any();
        public AutditLogs ToAudit()
        {
            var audit = new AutditLogs();
            audit.UserId = UserId;
            audit.Type = AuditType.ToString();
            audit.TableName = TableName;
            audit.DateTime =DateTime;
            audit.PrimaryKey = JsonConvert.SerializeObject(KeyValues);
            audit.OldValues = OldValues.Count == 0 ? "null" : JsonConvert.SerializeObject(OldValues);
            audit.NewValues = NewValues.Count == 0 ? "null" : JsonConvert.SerializeObject(NewValues);
            audit.AffectedColumns = ChangedColumns.Count == 0 ? "null" : JsonConvert.SerializeObject(ChangedColumns);
            return audit;
        }
    }
 }
