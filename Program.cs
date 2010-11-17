using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace UnfuddleBackupParser
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: UnfuddleBackupParser.exe InputFile OutputFolder [NameMappingsFile] [cleanup-events].\n");
                Console.WriteLine("Example: UnfuddleBackupParser.exe backup.xml \"X:\\Folder\\Output\" \"NameMappings.csv\" cleanup-events.\n");
                return;
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(args[0]);

            XmlElement accountElement = doc["account"];
            XmlElement peopleElement = accountElement["people"];
            People people = new People(peopleElement);

            string outputFolder = args[1];
            string peoplePath = Path.Combine(outputFolder, "persons.csv");

            if (args.Length >= 3)
                people.UseNameMappings(args[2]);

            people.Save(peoplePath);

            XmlElement projectsElement = accountElement["projects"];
            Projects projects = new Projects(projectsElement);

            string projectsPath = Path.Combine(outputFolder, "projects.csv");
            projects.SaveProjects(projectsPath);

            string areasPath = Path.Combine(outputFolder, "areas.csv");
            projects.SaveAreas(areasPath);

            string milestonesPath = Path.Combine(outputFolder, "milestones.csv");
            projects.SaveMilestones(milestonesPath);

            bool cleanupEvents = false;
            if (args.Length >= 4 && args[3] == "cleanup_events")
                cleanupEvents = true;

            string ticketsPath = Path.Combine(outputFolder, "tickets.xlsx");
            projects.SaveTickets(ticketsPath, people, cleanupEvents);
        }
    }
}
