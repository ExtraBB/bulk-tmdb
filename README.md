# Bulk tMDB Downloader

This C# application allows you to download tMDB data in bulk. I originally created this because I need to save/cache an offline copy of a specific subset of the tMDB database.

### What does it do?
 - It takes a comma-seperated values file (.csv) and searches a tMDB entry for each line.
 - It can search all kinds of tMDB entries, movies, tv, companies etc. It uses the multi search API function.
 - Results are outputted to seperate JSON files. (You can adjust this to save it in an SQL database for example)

### How do I get it to work?
- Enter your tMDB API key in the `Query.cs` file.
- Make sure your input file matches the specification. (See `AddSearchRequest()`)
- Run the application in Visual Studio.

### That's it! Have fun.
