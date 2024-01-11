# Contribution Guide

## Contribute Your Code

### Setup Snap.Hutao Project

1. Download and install [Visual Studio 2022 Community](https://visualstudio.microsoft.com/downloads/).
   - No need to select workloads; Visual Studio will handle it automatically.
   - Close Visual Studio Installer to ensure a smooth installation experience for workloads.
   - If using Visual Studio 2022 17.9 preview, skip step 5, as automatic extension installation is supported in this version.
2. Use git to clone the project `https://github.com/DGP-Studio/Snap.Hutao.git` to your local device.
3. Switch to the`develop` branch using git.
4. Open the project solution with your Visual Studio. Visual Studio will prompt you to install the necessary workloads, closing and reopening automatically.
5. (For Visual Studio 2022 17.8) Install the [Single-project MSIX Packaging Tools for VS 2022](https://marketplace.visualstudio.com/items?itemName=ProjectReunion.MicrosoftSingleProjectMSIXPackagingToolsDev17) provided by Microsoft in Visual Studio marketplace.
6. Open the project solution with your Visual Studio, and you are ready to go.

### Start Pull Request

- All code-related changes from authors' own branches are only allowed be merged to `develop` branch
- Please use [keywords](https://docs.github.com/en/get-started/writing-on-github/working-with-advanced-formatting/using-keywords-in-issues-and-pull-requests) to link your PR or commits with issues, so issues can be automatically closed once commits are merged into `main` branch.

### Test Binary Package

Once the code in updated in `develop` and `main` branches, an Azure Pipeline CI script will build the latest code to `Snap Hutao Alpha` package. Once the package is built, it will be released on [GitHub Release page](https://github.com/DGP-Studio/Snap.Hutao/releases) as a pre-released package. 

You need to install [Snap.Hutao.CI.cer](https://github.com/DGP-Studio/Snap.Hutao/releases/download/2023.10.3.1/Snap.Hutao.CI.cer) certificate to your local machine, and then install the msix package in the release. 

*If the latest release does not contains attached file, that means package is still in uploading process. 

## Start New Issue

To help users solve problems faster and increase developers' efficiency in solving problems, Snap Hutao provides detailed documentation to explain common problems and issue templates to guide users to report program problems by submitting issues.

Before submitting a new issue, you should check the following pages:

- [FAQ](https://hut.ao/advanced/FAQ.html) Document
- [Common Program Exceptions ](https://hut.ao/en/advanced/exceptions.html)Document
- [Current Opened BUG Report Issues](https://github.com/DGP-Studio/Snap.Hutao/issues?q=is%3Aissue+is%3Aopen+label%3ABUG)

When starting a new issue, please use the issue templates:

- Describe your issue in details to help developers to reproduce the issue
- Your description of reproduction should be a step-by-step story
- If your issue is about program crash
  - Remember to provide your Device ID
  - Check Windows Event Viewer, and attach associated `.NET Error` details in the issue body

## Document Modification

Snap Hutao Document site is stored in repository [DGP-Studio/Snap.Hutao.Docs](https://github.com/DGP-Studio/Snap.Hutao.Docs), you can process the following steps to test the site in your local device:

1. Download and install [NodeJS 18](https://nodejs.org/en/download/)
2. Clone the repository
3. Run `npm install` in the root directory of the document project
4. Run `npm run docs:dev` to start test on 8080 port

### Localization

Snap.Hutao.Docs project structure is designed as multiple languages site. Each language has its independent folder under `docs` directory.

**If you wish to add another language document, you can [start an issue in document repository](https://github.com/DGP-Studio/Snap.Hutao.Docs/issues) to ask developer to setup an environment for you, or you can process the following steps by yourself:** 

1. make a copy of `zh` folder, rename the new folder as the new language's code
2. Start your translation work in the new language folder
3. In `docs/.vuepress/sidebar` folder, duplicate `zh.ts` file
   1. Rename the file to `{language_code}.ts`
   2. In the line 4, change `/zh/` to `/{language_code}/`
   3. Translate all `text` field
4. In `docs/.vuepress/navbar` folder, duplicate `zh.ts` file
   1. Rename the file to `{language_code}.ts`
   2. Replace all `/zh/` to `/{language_code}/`
   3. Translate all `text` field
5. In `docs/.vuepress/config.ts`file, add your language information in `locales` and `plugins/docsearchPlugin/locales` dictionary
6. In `docs/.vuepress/theme.ts`file, add your language information in `locales` dictionary