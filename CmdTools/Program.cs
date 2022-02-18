// See https://aka.ms/new-console-template for more information

using CmdTools;
using CmdTools.Core.CmdMenuAndPages;

IIOWrapper ioWrapper = new IOWrapper();
ICmdWizard cmdWizard = new CmdToolsProgram(ioWrapper);

cmdWizard.Run();



