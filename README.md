Basketball Roster Manager
=========================
Basketball Roster Manager is a Windows desktop application that is made for use by public address announcers during basketball games, which can replace paper rosters and hand-written notes to keep up with fouls, among other helpful features.

Basketball Roster Manager is written in C#. It provides a modern UI, built on the idea that a UI is like a joke, if you have to explain it, it isn't good.

For the back-end part a local SQL database stores everything. 

## Screenshots 

[Not yet available; will post some soon.]

## Initial installation (quick & dirty documented)

First of all, you need to clone this project to your machine. After that, check Windows Updates to make sure that you have .NET Framework v 4.5 installed.

Second, you need compile the program.  Follow the instructions at https://msdn.microsoft.com/en-us/library/78f4aasd.aspx to compile the application.

## Upgrading

Upgrading to the latest GIT version of the Basketball Roster Manager is fairly easy. Update your local repository running `git pull`, recompile the application.  Since the database is stored in your roaming AppData folder, any data that you've saved will be accessible in newer versions.

## Developing

Developing on Basketball Roster Manager is fairly easy for anyone who's familiar with C# and WinForms. If you need something explained, then you probably shouldn't be contributing to this project.

## Other ways to contribute

If you can't help with the programming, feel free to contribute by using the application and opening Issues on GitHub when you find a bug or have a suggestion.  Alternatively, you could just help pay the bills by leaving some gratuity at https://gratipay.com/joshua_carroll/.
