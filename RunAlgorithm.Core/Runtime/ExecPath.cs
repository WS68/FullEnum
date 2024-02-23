﻿using System.Text;

namespace RunAlgorithm.Core.Runtime;

internal class ExecPath : IPathStep
{
    private readonly string[] _initString;

    public ExecPath(string[] initString)
    {
        _initString = initString;
    }

    public IPathStep? Parent => null;

    void IPathStep.WriteTo(StringBuilder sb)
    {
    }

    public override string ToString()
    {
        return "Root Path???";
    }
}