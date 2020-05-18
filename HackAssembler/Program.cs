using System;
using HackAssembler.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HackAssembler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            ServiceProvider serviceProvider = new ServiceCollection()
                  .AddLogging((ILoggingBuilder loggingBuilder) =>
                  {
                      loggingBuilder.AddConsole();
                  })
                  .Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information)
                  .AddSingleton<IParser, Parser>()
                  .BuildServiceProvider();
            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();
            logger.LogInformation("Starting application");
/*

show console name
show console app usage instructions

get filename from args
if no filename
    show error
    quit

for each line in the file
    parse and store parsing details in an array

PC = 0
parsingerrors=[]
for each parsedline in allparsedlines
    if parsedline is a label location e.g. "(LOOP)"
        if symbol exists in symbol table
            if symbol has no value
                store value of current PC
            else
                add parsingerror (duplicate label)
        else
            store symbol and value of current PC
    if parsedline is a reference to a label or a variable declaration e.g. "@counter" or "@LOOP"
        if symbol does not exist in symbol table
            store in symbol table with empty value
        PC++
    if parsedline is an a-instruction or c-instruction
        PC++
    if parsedline has syntax errors
        add parsingerror

if any parsingerrors
    output to the console
    quit
            

tempstorage = ???
foreach symbol in symbol table
    if symbol has no value
        set symbol to tempstorage
        tempstorage++

results = []
foreach parsedline in allparsedlines
    if parsedline is an a-instruction
        if value is a symbol
            results.push value of symbol
        else
            results.push a-instruction value
    if parsedline is a c-instruction
        results.push 111 & comp bits & dest bits & jump bits

foreach result in results
    write to output file
*/
        }
    }
}
