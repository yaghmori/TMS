using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Microsoft.AspNetCore.Authorization;
using TMS.Shared.Services;
using TMS.Shared.Constants;

namespace TMS.RootComponents.Pages
{

    public partial class Dashboard
    {


        public int UserCount { get; set; } = 0;
        public int TenantCount { get; set; } = 0;

        private bool _canViewDashboard = false;

        protected override async Task OnInitializedAsync()
        {
            _appState.SetAppTitle(_viewLocalizer["Dashboard"]);
            var userResponse = await _userDataService.GetUsersAsync();
            if (userResponse.Succeeded)
            {
                UserCount = userResponse.Data.Count();
            }
            var clientResponse = await _TenantDataService.GetTenantsAsync();
            if (clientResponse.Succeeded)
            {
                TenantCount = clientResponse.Data.Count();
            }
        }
        private int Index = -1; //default value cannot be 0 -> first selectedindex is 0.
        private int Index1 = -1; //default value cannot be 0 -> first selectedindex is 0.

        public List<ChartSeries> Series1 = new List<ChartSeries>()
    {
        new ChartSeries() { Name = "Series 1", Data = new double[] { 90, 79, 72, 69, 62, 62, 55, 65, 70 } },
        new ChartSeries() { Name = "Series 2", Data = new double[] { 10, 41, 35, 51, 49, 62, 69, 91, 148 } },
    };
        public string[] XAxisLabels1 = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep" };

        Random random = new Random();
        public void RandomizeData()
        {
            var new_series = new List<ChartSeries>()
        {
            new ChartSeries() { Name = "Series 1", Data = new double[9] },
            new ChartSeries() { Name = "Series 2", Data = new double[9] },
        };
            for (int i = 0; i < 9; i++)
            {
                new_series[0].Data[i] = random.NextDouble() * 100;
                new_series[1].Data[i] = random.NextDouble() * 100;
            }
            Series = new_series;
            StateHasChanged();
        }

        public List<ChartSeries> Series = new List<ChartSeries>()
    {
        new ChartSeries() { Name = "United States", Data = new double[] { 40, 20, 25, 27, 46, 60, 48, 80, 15 } },
        new ChartSeries() { Name = "Germany", Data = new double[] { 19, 24, 35, 13, 28, 15, 13, 16, 31 } },
        new ChartSeries() { Name = "Sweden", Data = new double[] { 8, 6, 11, 13, 4, 16, 10, 16, 18 } },
    };
        public string[] XAxisLabels = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep" };

        EarningReport[] earningReports = new EarningReport[] { new EarningReport { Name = "Lunees", Title = "Reactor Engineer", Avatar = "https://avatars2.githubusercontent.com/u/71094850?s=460&u=66c16f5bb7d27dc751f6759a82a3a070c8c7fe4b&v=4", Salary = ".99", Severity = Color.Success, SeverityTitle = "Low" }, new EarningReport { Name = "Mikes-gh", Title = "Developer", Avatar = "https://avatars.githubusercontent.com/u/16208742?s=120&v=4", Salary = "$19.12K", Severity = Color.Secondary, SeverityTitle = "Medium" }, new EarningReport { Name = "Garderoben", Title = "CSS Magician", Avatar = "https://avatars2.githubusercontent.com/u/10367109?s=460&amp;u=2abf95f9e01132e8e2915def42895ffe99c5d2c6&amp;v=4", Salary = "$1337", Severity = Color.Primary, SeverityTitle = "High" }, };
        class EarningReport
        {
            public string Avatar;
            public string Name;
            public string Title;
            public Color Severity;
            public string SeverityTitle;
            public string Salary;
        }
    }
}