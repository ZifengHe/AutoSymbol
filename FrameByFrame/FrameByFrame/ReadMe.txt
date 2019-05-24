Main Function Work Flow
Step 1 : Click WDI to load a huge CSV file in memory
Step 2 : Select one focus Country and accompanion countries 
Step 3 : Click RunFilter to filter out indicators that shows focus country unique
Step 4 : From many indicators pick one or more to be used in video
Step 5 : Click GenProj to generate csv file and project files

LifeCycle of RichTextBox control
1. Only RefreshView can draw RickTextBox
2. Add Edit Delete will work on RefreshView only
3. New CSV needs special logic (first render)
4. The combobox list maintained by (RefreshView and FirstRender)
