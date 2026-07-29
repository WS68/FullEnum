using RunAlgorithm.Core;

namespace SyncRootFolders.Steps;

public abstract class ContextStep : IStep
    {
    protected ContextStep(string name)
        {
        Name = name;
        }
    public string Name
        {
        get;
        }
    public abstract bool Execute(IContext context);
    public bool IsList => false;
    public IReadOnlyList<IStep> Variants => throw new NotImplementedException();
    }
public sealed class ReadIsRootStep : ContextStep
    {
    public ReadIsRootStep(string actor) : base("ReadIsRoot_" + actor) { }
    public override bool Execute(IContext context)
        {
        var actor = Name["ReadIsRoot_".Length..];
        var ctx = (Context)context;
        ctx.SaveIsRoot(actor, ctx.ReadIsRootImplicit());
        return true;
        }
    }
public sealed class OpenTransactionStep : ContextStep
    {
    public OpenTransactionStep(string actor) : base("OpenTransaction_" + actor) { }
    public override bool Execute(IContext context)
        {
        ((Context)context).GetTransaction(Name["OpenTransaction_".Length..]);
        return true;
        }
    }
public sealed class WriteInContentStep : ContextStep
    {
    private readonly bool _value;
    public WriteInContentStep(string actor, bool value) : base("WriteInContent_" + actor) { _value = value; }
    public override bool Execute(IContext context)
        {
        ((Context)context).WriteInContent(Name["WriteInContent_".Length..], _value);
        return true;
        }
    }
public sealed class FixIsRootStep : ContextStep
    {
    private readonly bool _target;
    public FixIsRootStep(string actor, bool target) : base("FixIsRoot_" + actor) { _target = target; }
    public override bool Execute(IContext context)
        {
        var actor = Name["FixIsRoot_".Length..];
        var ctx = (Context)context;
        if (ctx.SavedIsRoot(actor) != _target)
            ctx.WriteNeedCheck(actor, true);
        return true;
        }
    }
public sealed class CloseTransactionStep : ContextStep
    {
    public CloseTransactionStep(string actor) : base("CloseTransaction_" + actor) { }
    public override bool Execute(IContext context)
        {
        ((Context)context).CloseTransaction(Name["CloseTransaction_".Length..]);
        return true;
        }
    }

public sealed class ReadNeedCheckStep : ContextStep
    {
    public ReadNeedCheckStep(string actor) : base("ReadNeedCheck_" + actor) { }
    public override bool Execute(IContext context) => ((Context)context).ReadNeedCheckImplicit();
    }
public sealed class ReadTransactionValuesStep : ContextStep
    {
    public ReadTransactionValuesStep(string actor) : base("ReadTransactionValues_" + actor) { }
    public override bool Execute(IContext context)
        {
        var actor = Name["ReadTransactionValues_".Length..];
        var ctx = (Context)context;
        ctx.SaveIsRoot(actor, ctx.ReadIsRoot(actor));
        ctx.SaveInContent(actor, ctx.ReadInContent(actor));
        return true;
        }
    }
public sealed class CorrectIsRootStep : ContextStep
    {
    public CorrectIsRootStep(string actor) : base("CorrectIsRoot_" + actor) { }
    public override bool Execute(IContext context)
        {
        var actor = Name["CorrectIsRoot_".Length..];
        var ctx = (Context)context;
        if (ctx.SavedIsRoot(actor) == ctx.SavedInContent(actor))
            ctx.WriteIsRoot(actor, !ctx.SavedInContent(actor));
        return true;
        }
    }
public sealed class ResetNeedCheckStep : ContextStep
    {
    public ResetNeedCheckStep(string actor) : base("ResetNeedCheck_" + actor) { }
    public override bool Execute(IContext context)
        {
        ((Context)context).WriteNeedCheck(Name["ResetNeedCheck_".Length..], false);
        return true;
        }
    }
