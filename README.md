# Setup Script for the LotCom Printer application

[![Package Setup](https://github.com/LotCoM/LotCom-printer-setup/actions/workflows/cicd.yml/badge.svg?branch=develop)](https://github.com/LotCoM/LotCom-printer-setup/actions/workflows/cicd.yml)

Uses WiX and WiX-Sharp to create a custom MSI builder.

This builder's unpackaged release files can be side-loaded in the `Setup` folder in the LotCom Printer application project. From here, LotCom Printer's CI/CD pipeline will use the setup program to create and release an MSI package.

> [!Warning]
> When making changes to the Setup program, you must run `dotnet build` and migrate the newly generated release files to LotCom Printer's `Setup` directory. 
> 
> Otherwise, LotCom Printer will not run the modified Setup program in its CI/CD pipeline.
