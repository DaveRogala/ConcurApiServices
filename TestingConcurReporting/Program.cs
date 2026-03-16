
using ConcurExpense;
using ConcurExpense.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddConcurExpenseClient(options =>
	builder.Configuration.GetSection(ConcurOptions.SectionName).Bind(options));

using IHost host = builder.Build();

var service = host.Services.GetRequiredService<IConcurExpenseClient>();

if (service is not null)
{
	var reports = await service.GetReportsAsync(limit:100, modifiedDateAfter: DateTime.Today.AddDays(-1));

	List<EntryDto> entries = [];
	List<ItemizationDto> itemizations = [];
	List<AllocationDto> allocations = [];

	foreach (var report in reports.Where(r => r.ID is not null))
	{
		

		List<EntryDto> reportEntries = await service.GetEntriesAsync(report.ID, limit: 100);
		entries.AddRange(reportEntries);
		
		if(report.ID is not null && reportEntries.Any(e => e.HasItemizations))
		{
			itemizations.AddRange(await service.GetItemizationsAsync(report.ID, limit: 100));
		}
		
		allocations.AddRange(await service.GetAllocationsAsync(report.ID, limit: 100));
	}
}



string ReadValue(string message)
{
	string? inputString;
	bool validInput = false;
	do
	{
		Console.WriteLine(message);
		inputString = Console.ReadLine();
		validInput = !String.IsNullOrWhiteSpace(inputString);
	} while (!validInput);

	return inputString ?? "";
}