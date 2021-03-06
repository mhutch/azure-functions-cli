﻿using System;
using System.Threading.Tasks;
using Colors.Net;
using Fclp;
using Ignite.SharpNetSH;
using Azure.Functions.Cli.Common;
using Azure.Functions.Cli.Helpers;
using static Azure.Functions.Cli.Common.OutputTheme;

namespace Azure.Functions.Cli.Actions
{
    [Action(Name = "internal-use", ShowInHelp = false)]
    internal class InternalUseAction : BaseAction
    {
        public InternalAction Action { get; set; }
        public int Port { get; set; }

        public override ICommandLineParserResult ParseArgs(string[] args)
        {
            Parser
                .Setup<InternalAction>("action")
                .WithDescription(nameof(Action))
                .Callback(a => Action = a)
                .Required();
            Parser
                .Setup<int>("port")
                .WithDescription(nameof(Port))
                .Callback(p => Port = p);
            return Parser.Parse(args);
        }

        public override Task RunAsync()
        {
            if (Action == InternalAction.SetupUrlAcl)
            {
                if (!SecurityHelpers.IsAdministrator())
                {
                    ColoredConsole
                        .Error
                        .WriteLine(ErrorColor("When using the cert command you have to run as admin"));

                    Environment.Exit(ExitCodes.MustRunAsAdmin);
                }
                else
                {
                    SetupUrlAcl();
                }
            }
            return Task.CompletedTask;
        }

        private void SetupUrlAcl()
        {
            NetSH.CMD.Http.Add.UrlAcl($"http://+:{Port}/", $"{Environment.UserDomainName}\\{Environment.UserName}", null);
        }
    }

    internal enum InternalAction
    {
        None,
        SetupUrlAcl
    }
}
