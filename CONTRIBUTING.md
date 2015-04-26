# How to contribute to Hyprlinkr

Hyprlinkr is currently being developed in C# on .NET 4.5 using Visual Studio 2013 with xUnit.net as the unit testing framework. So far, all development has been done with TDD, so there's a high degree of code coverage, and it would be preferable to keep it that way.

## Dependencies

All binaries (such as xUnit.net) are included as NuGet packages in the source control repository under the \packages folder. All additional binaries not part of .NET 4.5 must also be added to the repository, so that it would be possible to pull down the repository and immediately be able to compile and run all tests.

Hyprlinkr does [not rely on NuGet Package Restore](http://blog.ploeh.dk/2014/01/29/nuget-package-restore-considered-harmful).

## Verification

There is currently a single solution to be found under the \Src folder, but be aware that the final verification step before pushing to the repository is to successfully build the BuildRelease.msbuild file. This can be done either from Bash (using build-release.sh) or PowerShell (using Build-Release.ps1).

As part of the verification build, Visual Studio Code Analysis is executed in a configuration that treats warnings as errors. No CA warnings should be suppressed unless the documented conditions for suppression are satisfied. Otherwise, a documented agreement between at least two active developers of the project should be reached to allow a suppression of a non-suppressable warning.

## Pull requests

Hyprlinkr is open source, and pull requests are welcome.

When developing for Hyprlinkr, please respect the coding style already present. Look around in the source code to get a feel for it.

Please keep line lengths under 120 characters. Line lengths over 120 characters doen't fit into the standard GitHub code listing window, so it requires vertical scrolling to review.

Also, please follow the [Open Source Contribution Etiquette](http://tirania.org/blog/archive/2010/Dec-31.html). Hyprlinkr is a fairly typical open source project: if you want to contribute, start by [creating a fork](http://help.github.com/fork-a-repo/) and [sending a pull request](http://help.github.com/send-pull-requests/) when you have something you wish to commit. When creating pull requests, please keep the Single Responsibility Principle in mind. A pull request that does a single thing very well is more likely to be accepted. See also the article [The Rules of the Open Road](http://blog.half-ogre.com/posts/software/rules-of-the-open-road) for more good tips on working with OSS and Pull Requests.

For complex pull requests, you are encouraged to first start a discussion on the [issue list](https://github.com/ploeh/Hyprlinkr/issues). This can save you time, because the Hyprlinkr regulars can help verify your idea, or point you in the right direction.

Some existing issues are marked with [the jump-in tag](http://nikcodes.com/2013/05/10/new-contributor-jump-in/). These are good candidates to attempt, if you are just getting started with Hyprlinkr.

When you submit a pull request, you can expect a response within a day or two. We (the maintainers of Hyprlinkr) have day jobs, so we may not be able to immediately review your pull request, but we do make it a priority. Also keep in mind that we may not be in your time zone.

Most likely, when we review pull requests, we will make suggestions for improvement. This is normal, so don't interpret it as though we don't like your pull request. On the contrary, if we agree on the overall goal of the pull request, we want to work *with* you to make it a success.