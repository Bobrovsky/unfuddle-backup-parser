Unfuddle Backup Parser parses backup.xml files that come from Unfuddle backups. Output files can be used for various purposes. We used output files to migrate data from Unfuddle to FogBugz.

You may want to check [FogBugz Importer](http://code.google.com/p/fogbugz-importer/) if you migrate to FogBugz, too.

Usage:<br>
<code>UnfuddleBackupParser.exe InputFile OutputFolder [NameMappingsFile] [cleanup-events]</code>

Example:<br>
<code>UnfuddleBackupParser.exe backup.xml "X:\Folder\Output\" "NameMappings.csv" cleanup-events</code>

The name mappings file is an optional simple comma-separated file in form<br>
<code>Person full name in Unfuddle, New name</code><br>
Unfuddle Backup Parser will use such mappings while producing files.<br>

The optional parameter <code>cleanup-events</code> can be used to produce smaller tickets.xlsx file. If <code>cleanup-events</code> was specified in command line then most audit trails from Unfuddle backup will not be processed and placed into output file.<br>
<br>
<a href='http://habreffect.ru/files/45e/7cfc5d87d/tickets_shot-highlighted.png'><img src='http://habreffect.ru/files/93e/4394d9e4b/tickets_shot-highlighted-preview.png' /></a>

The picture above shows structure of a tickets.xlsx. Parts highlighted with yellow will be absent in the file if <code>cleanup-events</code> was specified.<br>
<br>
After the parse the following output files with retrieved data are created:<br>
<b>tickets.xlsx</b> - contains data about tickets (cases) and events associated with them.<br>
areas.csv - contains list of project categories.<br>
milestones.csv - contains list of milestones in project.<br>
persons.csv - contains list of people involved in project<br>
projects.csv - contains name of the project<br>