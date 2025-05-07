using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Enums;

namespace Application.Features.Queries.ProjectNote
{
    public class GetAllProjectNoteViewModel
    {
        public int Id { get; set; }
        public NoteType NoteType { get; set; } 
        public string Note { get; set; }
        public DateTime Created { get; set; }
    }
}
