﻿using System;
using System.Linq;
using JasperFx.CodeGeneration.Frames;
using JasperFx.Core;

namespace JasperFx.CodeGeneration.Model;

public class ValueTypeReturnVariable : Variable
{

    public ValueTypeReturnVariable(Type returnType, Variable[] inner) : base(returnType)
    {
        Inners = inner.Select(x => new TupleVariable(x, ReturnAction.Initialize)).ToArray();
    }

    public override string Usage => "(" + Inners.Select(x => x.Usage()).Join(", ") + ")";
    
    public TupleVariable[] Inners { get; }

    public record TupleVariable(Variable Inner, ReturnAction Action)
    {
        public string Usage() => Action == ReturnAction.Initialize ? $"var {Inner.Usage}" : Inner.Usage;
    }

    public void AssignResultTo(int index, Variable variable)
    {
        Inners[index] = new TupleVariable(variable, ReturnAction.Assign);
    }
}