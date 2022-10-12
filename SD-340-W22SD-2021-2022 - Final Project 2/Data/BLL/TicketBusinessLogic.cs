using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_2021_2022___Final_Project_2.Data.DAL;
using SD_340_W22SD_2021_2022___Final_Project_2.Models;
using SD_340_W22SD_2021_2022___Final_Project_2.Models.ViewModels;
using System.Security.Claims;
using static Humanizer.In;

namespace SD_340_W22SD_2021_2022___Final_Project_2.Data.BLL
{
    public class TicketBusinessLogic
    {
        private TicketRepository _ticketRepository;
        private ProjectRepository _projectRepository;
        private UserManager<ApplicationUser> _userManager;
        public TicketBusinessLogic(TicketRepository ticketRepository,
            ProjectRepository projectRepository,
            UserManager<ApplicationUser> userManager)
        {
            _ticketRepository = ticketRepository;
            _userManager = userManager;
            _projectRepository = projectRepository;
        }
        public ICollection<Comment> CommentsOnTask(int ticketID)
        {            
            return _ticketRepository.Get(ticketID).Comment.ToList();
        }

        public async Task<AddToWatchListBLLViewModel> AddToWatchListAsync(ClaimsPrincipal user, int projectId, int ticketId)
        {
            try
            {
                ApplicationUser currentUser = await _userManager.GetUserAsync(user);

                Project project = _projectRepository.GetProjectWithDevelopers(projectId);
                
                Ticket ticket = _ticketRepository.GetTicketWithTaskWatchers(ticketId);

                if (project.Developers.FirstOrDefault(d => d.Id == currentUser.Id) == null)
                {
                    return new AddToWatchListBLLViewModel()
                    {
                        Unauthorized = true
                    };
                }

                if (ticket.TaskWatchers.FirstOrDefault(u => u.Id == currentUser.Id) == null)
                {
                    ticket.TaskWatchers.Add(currentUser);
                }
                else
                {
                    ticket.TaskWatchers.Remove(currentUser);
                }

                _ticketRepository.Update(ticket);

                _ticketRepository.Save();

                return new AddToWatchListBLLViewModel()
                {
                    Succeeded = true
                };
            }
            catch (Exception ex)
            {
                return new AddToWatchListBLLViewModel()
                {
                    Succeeded = true
                };
            }
        }

        public async Task<ToggleTicketBLLViewModel> ToggleTicketAsync(ClaimsPrincipal user , int ticketId)
        {
            try
            {
                ApplicationUser currentUser = await _userManager.GetUserAsync(user);

                Ticket ticket = _ticketRepository.GetTicketWithTaskOwners(ticketId);

                if (ticket.TaskOwners.FirstOrDefault(to => to.Id == currentUser.Id) == null)
                {
                    return new ToggleTicketBLLViewModel()
                    {
                        Unauthorized = true
                    };
                }

                ticket.Completed = !ticket.Completed;

                _ticketRepository.Update(ticket);

                _ticketRepository.Save();

                return new ToggleTicketBLLViewModel()
                {
                    Succeeded = true
                };
            }
            catch (Exception ex)
            {
                return new ToggleTicketBLLViewModel()
                {
                    Succeeded = false
                };
            }
        }

        public async Task<ChangeRequiredHoursAsyncBLLViewModel> ChangeRequiredHoursAsync(ClaimsPrincipal user, int ticketId, int hours)
        {
            try
            {
                ApplicationUser currentUser = await  _userManager.GetUserAsync(user);
                Ticket ticket = _ticketRepository.GetTicketWithTaskOwners(ticketId);

                if (ticket.TaskOwners.FirstOrDefault(to => to.Id == currentUser.Id) == null)
                {
                    return new ChangeRequiredHoursAsyncBLLViewModel()
                    {
                        Unauthorized = true
                    };
                };

                ticket.Hours = hours;
                
                _ticketRepository.Update(ticket);
                
                _ticketRepository.Save();

                return new ChangeRequiredHoursAsyncBLLViewModel()
                {
                    Succeeded = true
                };
            }
            catch (Exception ex)
            {
                return new ChangeRequiredHoursAsyncBLLViewModel()
                {
                    Succeeded = false
                };
            }
        }
    }
}
