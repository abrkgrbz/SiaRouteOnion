using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.ComplexModel;
using Application.DTOs.User;
using Application.Features.Commands.PrintStudy.CreatePrintStudy;
using Application.Features.Commands.Project.CreateProject;
using Application.Features.Commands.ProjectMethod.CreateProjectMethod;
using Application.Features.Commands.ProjectNote;
using Application.Features.Commands.ProjectProcess.UpdateCompletedProcess;
using Application.Features.Commands.UserProject.CreateUserProject;
using Application.Features.Queries.PrintStudy;
using Application.Features.Queries.Project.GetAllProject;
using Application.Features.Queries.Project.GetProjectDetails;
using Application.Features.Queries.ProjectMethod.GetProjectMethod;
using Application.Features.Queries.ProjectNote;
using Application.Features.Queries.Scm.GetAllScm;
using Application.Features.Queries.User.GetAllUser;
using Application.Features.Queries.User.GetUserByName;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class GeneralProfile:Profile
    {
        public GeneralProfile()
        {
            CreateMap<CreatePrintStudyCommand, PrintStudy>().ReverseMap();
            CreateMap<CreateProjectCommand, Project>().ReverseMap(); 
            CreateMap<ProjectList, GetAllProjectVM>().ReverseMap(); 
         
            CreateMap<Project, ProjectDetailsViewModel>()
				 .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom<UserNameResolverViewModel>());
            CreateMap<UserResponse, GetUserByNameViewModel>();
            CreateMap<UserResponse, GetAllUserQueryViewModel>();
            CreateMap<PrintStudy, PrintStudyDetailViewModel>().ReverseMap();
            CreateMap<SCM, GetAllScmViewModel>().ReverseMap();
            CreateMap<ProjectProcessesDetailViewModel, ProjectProcess>().ReverseMap();
            CreateMap<CreateProjectNoteCommand, ProjectNotes>().ReverseMap();
            CreateMap<GetAllProjectNoteViewModel, ProjectNotes>().ReverseMap();
            CreateMap<GetProjectMethodViewModel, ProjectMethods>().ReverseMap();
            CreateMap<CreateProjectMethodCommand, ProjectMethods>().ReverseMap();
        }

        #region Resolvers

        public class UserNameResolver : IValueResolver<Project, GetAllProjectVM, string>
        {
	        private readonly IAccountService _userService;

	        public UserNameResolver(IAccountService userService)
	        {
		        _userService = userService;
	        }

	        public string Resolve(Project source, GetAllProjectVM destination, string destMember, ResolutionContext context)
	        {
		        var user = _userService.GetByUserProfile(source.CreatedBy);
		        return NameCensoring(user.Result.FirstName + " " + user.Result.LastName);
	        }
        }


        public class UserNameResolverViewModel : IValueResolver<Project, ProjectDetailsViewModel, string>
        {
	        private readonly IAccountService _userService;

	        public UserNameResolverViewModel(IAccountService userService)
	        {
		        _userService = userService;
	        }

	        public string Resolve(Project source, ProjectDetailsViewModel destination, string destMember, ResolutionContext context)
	        {
		        var user = _userService.GetByUserProfile(source.CreatedBy);
		        return user.Result.FirstName + " " + user.Result.LastName;
	        }
        }

       
        #endregion

        #region Utils

        public static string NameCensoring(string name)
        {
            string[] words = name.Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 2)
                {
                    words[i] = words[i].Substring(0, 2) + new string('*', words[i].Length - 2);
                }
                else
                {
                    words[i] = new string('*', words[i].Length);
                }
            }

            return string.Join(' ', words);
        }

        #endregion
    }
}
