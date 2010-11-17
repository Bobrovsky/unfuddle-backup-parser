using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace UnfuddleBackupParser
{
    class People
    {
        struct Person
        {
            public string id;
            public string userName;
            public string firstName;
            public string lastName;
        }

        SortedDictionary<int, Person> m_people;
        Dictionary<string, string> m_nameMappings = new Dictionary<string, string>();

        public People(XmlElement element)
        {
            m_people = new SortedDictionary<int, Person>();
            parse(element);
        }

        public void UseNameMappings(string path)
        {
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(path, Encoding.Unicode))
            {
                while (!sr.EndOfStream)
                {
                    sb.Append(sr.ReadLine());
                    sb.Append(',');
                }
            }

            string csv = sb.ToString();
            string[] splitResult = csv.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            m_nameMappings.Clear();
            for (int i = 0; i < splitResult.Length; )
            {
                string name = splitResult[i++].Trim();
                string newName = splitResult[i++].Trim();
                m_nameMappings.Add(name, newName);
            }
        }

        public string GetPersonName(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            int nameId = Convert.ToInt32(id);
            Person p = m_people[nameId];
            string name = p.firstName + " " + p.lastName;
            name = name.Trim();

            if (m_nameMappings.ContainsKey(name))
                return m_nameMappings[name];

            return name;
        }

        public void Save(string path)
        {
            StringBuilder sb = new StringBuilder();
            addPerson(sb, "id", "userName", "fullName");

            foreach (KeyValuePair<int, Person> pair in m_people)
            {
                Person person = pair.Value;
                addPerson(sb, person.id, person.userName, GetPersonName(person.id));
            }

            File.WriteAllText(path, sb.ToString(), Encoding.Unicode);
        }

        private void parse(XmlElement element)
        {
            foreach (XmlNode node in element.ChildNodes)
            {
                Person person = new Person();

                foreach (XmlNode child in node.ChildNodes)
                {
                    switch (child.Name)
                    {
                        case "id":
                            person.id = child.InnerText;
                            break;

                        case "username":
                            person.userName = child.InnerText;
                            break;

                        case "first-name":
                            person.firstName = child.InnerText;
                            break;

                        case "last-name":
                            person.lastName = child.InnerText;
                            break;
                    }
                }

                m_people.Add(Convert.ToInt32(person.id), person);
            }
        }

        private void addPerson(StringBuilder sb, string id, string userName, string fullName)
        {
            sb.Append(id);
            sb.Append(',');
            sb.Append(userName);
            sb.Append(',');
            sb.Append(fullName);
            sb.Append(',');
            sb.Append('\n');
        }
    }
}
