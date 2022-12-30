# About

MastodonFollowerTimes is a Microsoft Windows Desktop Application that you can use to find the best times to post on Mastodon. Simply run the application, fill in your instance name, application token (see below), and account name, and click "Find Best Times".

You'll see a list of the best hours to post, and if you expand one of those lines, you'll see a list of the best minutes to post inside those hours.

## How it Works

When you click "Find Best Times", the application gets a list of your 80 most recent followers. For each of those followers, it gets a list of their 40 most recent posts. It then aggregates that data into a number of posts per minute.

The logic here is that the times when most of your followers post will be the times they're most likely to be online, and therefore the times when they're most likely to see your own posts!

# Installation

There is no installation package. All you need to do is head on over to <a href="https://github.com/GrahamDo/MastodonFollowerTimes/releases" target="_blank">Releases</a> and download the most recent zip file.

Create a directory on your computer (such as "C:\MastodonFollowerTimes") and extract the contents of the zip file into that directory.

Then, simply double-click the exe to run the program.

Note that you'll need to make sure your user has write access to that directory, because the application saves a file called settings.json to remember your settings.

Also note that you need the .NET 7 Desktop Runtime installed on your computer to be able to run the application. If you don't have it, Windows will offer to download it for you the first time you run the application.

Finally, you might want to create a shortcut to the application exe and copy it (the shortcut, not the exe) to a convenient location like your start menu or desktop.

**Important: the first time you run MastodonFollowerTimes—either the first time ever, or the first time after upgrading—you may get a warning from Windows that the application isn't signed. If so, click "Run Anyway" (you might need to click "Advanced" first, depending on your version of WIndows).**

## Creating a Token

Before you can run this application, you'll need to create a Mastodon app and get a token. Follow these steps:

1. Open a new browser tab and login in to your account at your Mastodon instance.
2. Click the gear icon (Preferences) icon to access your Mastodon preferences.
3. Click Development in the left menu.
4. Click the New Application button.
5. Enter an Application Name of your choice (for example, "MastodonFollowerTimes").
6. Make sure you select the scopes that allow reading all data, followers, and statuses.
7. Click the Submit button.
8. Click the hyperlinked name of your application.
9. Copy the token and paste it into the "Token" field of the application.

# Questions / Comments / Feedback?

For tips and tricks, and to stay updated, <a rel="me" href="https://c.im/@FollowerTimesApp">Follow us on Mastodon</a>.

If you think you've found a bug, or have a suggestion for a new feature, click the <a href="https://github.com/GrahamDo/MastodonFollowerTimes/issues" target="_blank">Issues</a> tab, and create a new issue (after searching open issues to make sure yours hasn't already been reported, of course).
