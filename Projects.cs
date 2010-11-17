using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Web;

namespace UnfuddleBackupParser
{
    class Projects
    {
        struct Project
        {
            public string id;
            public string title;
            public Components components;
            public Milestones milestones;
            public Tickets tickets;
        }

        SortedDictionary<int, Project> m_projects;

        public Projects(XmlElement element)
        {
            m_projects = new SortedDictionary<int, Project>();
            parse(element);
        }

        public void SaveProjects(string path)
        {
            StringBuilder sb = new StringBuilder();
            addProject(sb, "id", "name");

            foreach (KeyValuePair<int, Project> pair in m_projects)
            {
                Project project = pair.Value;
                addProject(sb, project.id, project.title);
            }

            File.WriteAllText(path, sb.ToString(), Encoding.Unicode);
        }

        public void SaveAreas(string path)
        {
            StringBuilder sb = new StringBuilder();
            
            foreach (KeyValuePair<int, Project> pair in m_projects)
            {
                Project project = pair.Value;
                if (project.components != null)
                    project.components.AddAllAreas(sb);
            }

            File.WriteAllText(path, sb.ToString(), Encoding.Unicode);
        }

        public void SaveMilestones(string path)
        {
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<int, Project> pair in m_projects)
            {
                Project project = pair.Value;
                if (project.milestones != null)
                    project.milestones.AddAllMilestones(sb);
            }

            File.WriteAllText(path, sb.ToString(), Encoding.Unicode);
        }

        public void SaveTickets(string path, People people, bool cleanupEvents)
        {
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook workbook = app.Workbooks.Add(1);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];

            foreach (KeyValuePair<int, Project> pair in m_projects)
            {
                Project project = pair.Value;
                Tickets tickets = project.tickets;
                if (tickets != null)
                    tickets.Save(worksheet, project.title, project.components, project.milestones, people, cleanupEvents);
            }

            workbook.SaveAs(path);
            workbook.Close();
        }

        private void parse(XmlElement element)
        {
            foreach (XmlNode node in element.ChildNodes)
            {
                Project project = new Project();

                foreach (XmlNode child in node.ChildNodes)
                {
                    switch (child.Name)
                    {
                        case "id":
                            project.id = HttpUtility.HtmlDecode(child.InnerText);
                            break;

                        case "title":
                            project.title = HttpUtility.HtmlDecode(child.InnerText);
                            break;

                        case "components":
                            project.components = new Components(child as XmlElement);
                            break;

                        case "milestones":
                            project.milestones = new Milestones(child as XmlElement);
                            break;

                        case "tickets":
                            project.tickets = new Tickets(child as XmlElement);
                            break;
                    }
                }

                m_projects.Add(Convert.ToInt32(project.id), project);
            }
        }

        private static void addProject(StringBuilder sb, string id, string name)
        {
            sb.Append(id);
            sb.Append(',');
            sb.Append(name);
            sb.Append('\n');
        }
    }
}
