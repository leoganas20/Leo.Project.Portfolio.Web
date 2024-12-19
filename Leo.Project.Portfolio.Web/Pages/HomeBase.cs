using Leo.Project.Portfolio.Web.Pages.EmailSender.Model;
using Leo.Project.Portfolio.Web.Services;
using Leo.Projects.Portfolio.Web.Pages.ComponentPageBase;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using static MudBlazor.CategoryTypes;

namespace Leo.Project.Portfolio.Web.Pages;

public class HomeBase : ComponentPageBase
{
    protected EmailRequestParameterable EmailForm = new EmailRequestParameterable();
    protected MudForm form = new MudForm();
    protected bool IsFormVisible = false;
    protected bool ISuccess = false;
    protected bool IsSending = false;
    protected string[] FormErrors { get; set; }

    [Inject] public required RequestEmailService RequestEmail { get; set; }
    [Inject] public ISnackbar snackbar { get; set; }
    
    protected async override Task OnInitializedAsync()
    {
        IsVisible = true;

        await Task.Delay(3000);

        IsVisible = false;
    }

    public void ToggleFormVisibility()
    {
        IsFormVisible = !IsFormVisible;
    }
    
    protected void OnOverlayClosed()
    {

    }
    protected async Task SendEmail(EmailRequestParameterable model)
    {
        await form.Validate();

        if (!form.IsValid)
        {
            return;
        }
        IsVisible = true;

        if (string.IsNullOrEmpty(model.SenderEmail) || string.IsNullOrEmpty(model.Subject) || string.IsNullOrEmpty(model.Body))
        {
            snackbar.Add("Please enter the required fields!", Severity.Error);
            return;
        }
        var res = await RequestEmail.SendEmail(model.SenderEmail, model.Subject, model.Body);

        if (!string.IsNullOrEmpty(res))
        {
            snackbar.Add("Email Sent", Severity.Success, config =>
            {
                config.RequireInteraction = true;
                config.ShowCloseIcon = true;
                
            });

        }

        IsVisible = false;
        IsFormVisible = false;
    }
}