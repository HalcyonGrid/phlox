using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Antlr.Runtime;

using Halcyon.Phlox.Types;

namespace Halcyon.Phlox.Compiler
{
    public class SymbolTable
    {
        public static readonly BuiltInTypeSymbol INT
            = new BuiltInTypeSymbol("integer", (int)VarType.Integer);

        public static readonly BuiltInTypeSymbol FLOAT
            = new BuiltInTypeSymbol("float", (int)VarType.Float);

        public static readonly BuiltInTypeSymbol VECTOR
            = new BuiltInTypeSymbol("vector", (int)VarType.Vector);

        public static readonly BuiltInTypeSymbol ROTATION
            = new BuiltInTypeSymbol("rotation", (int)VarType.Rotation);

        public static readonly BuiltInTypeSymbol LIST
            = new BuiltInTypeSymbol("list", (int)VarType.List);

        public static readonly BuiltInTypeSymbol KEY
            = new BuiltInTypeSymbol("key", (int)VarType.Key);

        public static readonly BuiltInTypeSymbol STRING
            = new BuiltInTypeSymbol("string", (int)VarType.String);

        public static readonly BuiltInTypeSymbol VOID
            = new BuiltInTypeSymbol("void", (int)VarType.Void);

            /** Map LHS op RHS to result type (VOID implies illegal) */
        public static readonly ISymbolType[,] additionResultType = new ISymbolType[,] {
            /*          int     float   vector      rotation    list        key     string      void*/
            /*int*/     {INT,   FLOAT,  VOID,       VOID,       LIST,       VOID,   VOID,       VOID},
            /*float*/   {FLOAT, FLOAT,  VOID,       VOID,       LIST,       VOID,   VOID,       VOID},
            /*vector*/  {VOID,  VOID,   VECTOR,     VOID,       LIST,       VOID,   VOID,       VOID},
            /*rotation*/{VOID,  VOID,   VOID,       ROTATION,   LIST,       VOID,   VOID,       VOID},
            /*list*/    {LIST,  LIST,   LIST,       LIST,       LIST,       LIST,   LIST,       VOID},
            /*key*/     {VOID,  VOID,   VOID,       VOID,       LIST,       VOID,   VOID,       VOID},
            /*string*/  {VOID,  VOID,   VOID,       VOID,       LIST,       VOID,   STRING,     VOID},
            /*void*/    {VOID,  VOID,   VOID,       VOID,       VOID,       VOID,   VOID,       VOID}
        };

        public static readonly ISymbolType[,] subtractionResultType = new ISymbolType[,] {
            /*          int     float   vector      rotation    list        key     string      void*/
            /*int*/     {INT,   FLOAT,  VOID,       VOID,       VOID,       VOID,   VOID,       VOID},
            /*float*/   {FLOAT, FLOAT,  VOID,       VOID,       VOID,       VOID,   VOID,       VOID},
            /*vector*/  {VOID,  VOID,   VECTOR,     VOID,       VOID,       VOID,   VOID,       VOID},
            /*rotation*/{VOID,  VOID,   VOID,       ROTATION,   VOID,       VOID,   VOID,       VOID},
            /*list*/    {VOID,  VOID,   VOID,       VOID,       VOID,       VOID,   VOID,       VOID},
            /*key*/     {VOID,  VOID,   VOID,       VOID,       VOID,       VOID,   VOID,       VOID},
            /*string*/  {VOID,  VOID,   VOID,       VOID,       VOID,       VOID,   VOID,       VOID},
            /*void*/    {VOID,  VOID,   VOID,       VOID,       VOID,       VOID,   VOID,       VOID}
        };

        public static readonly ISymbolType[,] multiplicationResultType = new ISymbolType[,] {
            /*          int         float   vector      rotation    list        key     string      void*/
            /*int*/     {INT,       FLOAT,  VECTOR,     VOID,       VOID,       VOID,   VOID,       VOID},
            /*float*/   {FLOAT,     FLOAT,  VECTOR,     VOID,       VOID,       VOID,   VOID,       VOID},
            /*vector*/  {VECTOR,    VECTOR, FLOAT,      VECTOR,     VOID,       VOID,   VOID,       VOID},
            /*rotation*/{VOID,      VOID,   VOID,       ROTATION,   VOID,       VOID,   VOID,       VOID},
            /*list*/    {VOID,      VOID,   VOID,       VOID,       VOID,       VOID,   VOID,       VOID},
            /*key*/     {VOID,      VOID,   VOID,       VOID,       VOID,       VOID,   VOID,       VOID},
            /*string*/  {VOID,      VOID,   VOID,       VOID,       VOID,       VOID,   VOID,       VOID},
            /*void*/    {VOID,      VOID,   VOID,       VOID,       VOID,       VOID,   VOID,       VOID}
        };

        public static readonly ISymbolType[,] divisionResultType = new ISymbolType[,] {
            /*          int         float   vector      rotation    list        key     string      void*/
            /*int*/     {INT,       FLOAT,  VOID,       VOID,       VOID,       VOID,   VOID,       VOID},
            /*float*/   {FLOAT,     FLOAT,  VOID,       VOID,       VOID,       VOID,   VOID,       VOID},
            /*vector*/  {VECTOR,    VECTOR, VOID,       VECTOR,     VOID,       VOID,   VOID,       VOID},
            /*rotation*/{VOID,      VOID,   VOID,       ROTATION,   VOID,       VOID,   VOID,       VOID},
            /*list*/    {VOID,      VOID,   VOID,       VOID,       VOID,       VOID,   VOID,       VOID},
            /*key*/     {VOID,      VOID,   VOID,       VOID,       VOID,       VOID,   VOID,       VOID},
            /*string*/  {VOID,      VOID,   VOID,       VOID,       VOID,       VOID,   VOID,       VOID},
            /*void*/    {VOID,      VOID,   VOID,       VOID,       VOID,       VOID,   VOID,       VOID}
        };

        public static readonly ISymbolType[,] modResultType = new ISymbolType[,] {
            /*          int     float   vector      rotation    list        key     string      void*/
            /*int*/     {INT,   VOID,   VOID,       VOID,       VOID,       VOID,   VOID,       VOID},
            /*float*/   {VOID,  VOID,   VOID,       VOID,       VOID,       VOID,   VOID,       VOID},
            /*vector*/  {VOID,  VOID,   VECTOR,     VOID,       VOID,       VOID,   VOID,       VOID},
            /*rotation*/{VOID,  VOID,   VOID,       VOID,       VOID,       VOID,   VOID,       VOID},
            /*list*/    {VOID,  VOID,   VOID,       VOID,       VOID,       VOID,   VOID,       VOID},
            /*key*/     {VOID,  VOID,   VOID,       VOID,       VOID,       VOID,   VOID,       VOID},
            /*string*/  {VOID,  VOID,   VOID,       VOID,       VOID,       VOID,   VOID,       VOID},
            /*void*/    {VOID,  VOID,   VOID,       VOID,       VOID,       VOID,   VOID,       VOID}
        };

        public static readonly ISymbolType[,] shiftResultType = new ISymbolType[,] {
            /*          int     float   vector      rotation    list        key     string      void*/
            /*int*/     {INT,   VOID,   VOID,       VOID,       VOID,       VOID,   VOID,       VOID},
            /*float*/   {VOID,  VOID,   VOID,       VOID,       VOID,       VOID,   VOID,       VOID},
            /*vector*/  {VOID,  VOID,   VOID,       VOID,       VOID,       VOID,   VOID,       VOID},
            /*rotation*/{VOID,  VOID,   VOID,       VOID,       VOID,       VOID,   VOID,       VOID},
            /*list*/    {VOID,  VOID,   VOID,       VOID,       VOID,       VOID,   VOID,       VOID},
            /*key*/     {VOID,  VOID,   VOID,       VOID,       VOID,       VOID,   VOID,       VOID},
            /*string*/  {VOID,  VOID,   VOID,       VOID,       VOID,       VOID,   VOID,       VOID},
            /*void*/    {VOID,  VOID,   VOID,       VOID,       VOID,       VOID,   VOID,       VOID}
        };

        /** Indicate whether a type needs a promotion to a wider type.
            *  If not null, implies promotion required.  Null does NOT imply
            *  error--it implies no promotion.
            */
        public static readonly ISymbolType[,] promoteFromTo = new ISymbolType[,] {
            /*          int     float   vector      rotation    list        key     string      void*/
            /*int*/     {null,  FLOAT,  null,       null,       null,       null,   null,       null},
            /*float*/   {null,  null,   null,       null,       null,       null,   null,       null},
            /*vector*/  {null,  null,   null,       null,       null,       null,   null,       null},
            /*rotation*/{null,  null,   null,       null,       null,       null,   null,       null},
            /*list*/    {null,  null,   null,       null,       null,       null,   null,       null},
            /*key*/     {null,  null,   null,       null,       null,       null,   STRING,     null},
            /*string*/  {null,  null,   null,       null,       null,       KEY,    null,       null},
            /*void*/    {null,  null,   null,       null,       null,       null,   null,       null}
        };

        /** Controls which types may be casted to another type. VOID implies casting error
            */
        public static readonly ISymbolType[,] castFromTo = new ISymbolType[,] {
            /*          int     float   vector      rotation    list        key     string      void*/
            /*int*/     {INT,   FLOAT,  VOID,       VOID,       LIST,       VOID,   STRING,     VOID},
            /*float*/   {INT,   FLOAT,  VOID,       VOID,       LIST,       VOID,   STRING,     VOID},
            /*vector*/  {VOID,  VOID,   VECTOR,     VOID,       LIST,       VOID,   STRING,     VOID},
            /*rotation*/{VOID,  VOID,   VOID,       ROTATION,   LIST,       VOID,   STRING,     VOID},
            /*list*/    {VOID,  VOID,   VOID,       VOID,       LIST,       VOID,   STRING,     VOID},
            /*key*/     {VOID,  VOID,   VOID,       VOID,       LIST,       KEY,    STRING,     VOID},
            /*string*/  {INT,   FLOAT,  VECTOR,     ROTATION,   LIST,       KEY,    STRING,     VOID},
            /*void*/    {VOID,  VOID,   VOID,       VOID,       VOID,       VOID,   VOID,       VOID}
        };

        private ILSLListener _listener = new DefaultLSLListener();
        public ILSLListener StatusListener
        {
            get
            {
                return _listener;
            }

            set
            {
                _listener = value;
            }
        }



        public readonly ISymbolType[] indexToType = { INT, FLOAT, VECTOR, ROTATION, LIST, KEY, STRING, VOID };

        private GlobalScope _globals = new GlobalScope();
        public GlobalScope Globals
        {
            get
            {
                return _globals;
            }
        }

        private SupportedEventList _supportedEvents = new SupportedEventList();

        private ITokenStream _tokens;


        public IEnumerable<StateSymbol> States
        {
            get
            {
                foreach (Symbol sym in _globals.Symbols.Where(p => p is StateSymbol))
                {
                    yield return (StateSymbol)sym;
                }
            }
        }

        public int NumGlobals
        {
            get
            {
                return _globals.Symbols.Count(p => ((p is VariableSymbol) && ! (p is ConstantSymbol)));
            }
        }

            
        public SymbolTable(ITokenStream tokens, IEnumerable<FunctionSig> sysCalls, 
            IEnumerable<ConstantSymbol> constants)
        {
            _tokens = tokens;
            this.InitTypeSystem(sysCalls, constants);
        }

        private void InitTypeSystem(IEnumerable<FunctionSig> systemFunctions,
            IEnumerable<ConstantSymbol> constants)
        {
            foreach (Symbol symbol in indexToType)
            {
                if (symbol != null) _globals.Define(symbol);
            }

            if (systemFunctions != null)
            {
                foreach (FunctionSig fn in systemFunctions)
                {
                    MethodSymbol sysMethod = new MethodSymbol(fn.FunctionName, indexToType[(int)fn.ReturnType], _globals);
                    sysMethod.IsSyscall = true;

                    for (int i = 0; i < fn.ParamNames.Length; i++)
                    {
                        sysMethod.Members.Add(fn.ParamNames[i], new VariableSymbol(fn.ParamNames[i], indexToType[(int)fn.ParamTypes[i]]));
                    }

                    _globals.Define(sysMethod);
                }
            }

            if (constants != null)
            {
                foreach (ConstantSymbol var in constants)
                {
                    _globals.Define(var);
                }
            }
        }

        public Symbol EnsureResolve(LSLAst node, IScope scope, string symName)
        {
            Symbol sym = scope.Resolve(symName);

            if (sym == null)
            {
                _listener.Error("line " + node.Token.Line + ":" +
                    node.Token.CharPositionInLine + " Undefined symbol '" + symName + "'");

                return null;
            }

            //globals are always ok to ref, so skip the rest of the checks if this is a global
            if (sym.Scope == Globals)
            {
                return sym;
            }

            //consts are always ok to ref
            if (sym is ConstantSymbol)
            {
                return sym;
            }

            if (sym.Def == null)
            {
                _listener.Error(String.Format("INTERNAL COMPILER ERROR: Symbol definition not set: {0}", sym));
                return sym;
            }

            //if the symbol is local, it cant be used before it's defined
            if (sym.Def.TokenStartIndex > node.TokenStartIndex)
            {
                //if we are in a local scope, check if there is a version of this symbol in the function (parameter) scope
                //then check if there is a version of this symbol on the global scope
                Symbol methodSymbol = FindSymbolInMethodScope(sym, scope);
                Symbol globalSymbol = Globals.Resolve(symName);

                if (methodSymbol == null && globalSymbol == null)
                {
                    //there is no global by this name, it's only defined locally and it's defined after
                    //its use
                    _listener.Error("line " + node.Token.Line + ":" +
                        node.Token.CharPositionInLine + " Symbol '" + symName + "' can not be used before it is defined");
                }

                if (methodSymbol != null) sym = methodSymbol;
                else sym = globalSymbol;
            }
            //also if this symbol is local, it can not be defined in the root of the expression from which it is being used
            else if (IsChildNodeOf((LSLAst)sym.Def.GetAncestor(LSLParser.VAR_DECL), node))
            {
                //the symbol we found is a parent of the declaration and is therefore not valid for use
                //check parent scopes for a valid symbol
                Symbol methodSymbol = FindSymbolInMethodScope(sym, scope);
                Symbol globalSymbol = Globals.Resolve(symName);

                if (methodSymbol == null && globalSymbol == null)
                {
                    //there is no global by this name, it's only defined locally and it's defined after
                    //its use
                    _listener.Error("line " + node.Token.Line + ":" +
                        node.Token.CharPositionInLine + " Symbol '" + symName + "' can not be used before it is defined");
                }

                if (methodSymbol != null) sym = methodSymbol;
                else sym = globalSymbol;
            }

            return sym;
        }

        private bool IsChildNodeOf(LSLAst sym, LSLAst node)
        {
            if (sym == null || sym.Children == null)
            {
                return false;
            }

            foreach (Antlr.Runtime.Tree.ITree childNode in sym.Children)
            {
                if (childNode == node)
                {
                    return true;
                }

                if (IsChildNodeOf((LSLAst)childNode, node))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasUnknownTypes(LSLAst lhs, LSLAst rhs)
        {
            if (lhs.evalType == null || rhs.evalType == null)
            {
                _listener.Error("line " + lhs.Token.Line + ":" +
                    lhs.Token.CharPositionInLine + " Invalid operation. At least one type is unknown");
                return true;
            }

            return false;
        }

        public bool HasUnknownType(LSLAst lhs)
        {
            if (lhs.evalType == null)
            {
                _listener.Error("line " + lhs.Token.Line + ":" +
                    lhs.Token.CharPositionInLine + " Invalid operation. At least one type is unknown");
                return true;
            }

            return false;
        }

        public ISymbolType Bop(LSLAst bop, LSLAst lhs, LSLAst rhs)
        {
            ISymbolType[,] symTable;
            string op = bop.Token.Text;

            symTable = FindOperationTable(lhs, op);
            if (symTable == null)
            {
                return VOID;
            }

            if (HasUnknownTypes(lhs, rhs))
            {
                return VOID;
            }

            ISymbolType symType = symTable[lhs.evalType.TypeIndex, rhs.evalType.TypeIndex];
            if (symType == VOID)
            {
                _listener.Error("line " + lhs.Token.Line + ":" +
                    lhs.Token.CharPositionInLine + " '" + op + "' is not a valid operation between " +
                    lhs.evalType.Name + " and " + rhs.evalType.Name);
                return VOID;
            }

            return symType;
        }

        private ISymbolType[,] FindOperationTable(LSLAst lhs, string op)
        {
            ISymbolType[,] symTable;
            switch (op)
            {
                case "+":
                case "+=":
                    symTable = additionResultType;
                    break;

                case "-":
                case "-=":
                    symTable = subtractionResultType;
                    break;

                case "*":
                case "*=":
                    symTable = multiplicationResultType;
                    break;

                case "/":
                case "/=":
                    symTable = divisionResultType;
                    break;

                case "%":
                case "%=":
                    symTable = modResultType;
                    break;

                case "<<":
                case ">>":
                case ">>=":
                case "<<=":
                    symTable = shiftResultType;
                    break;

                default:
                    _listener.Error("line " + lhs.Token.Line + ":" +
                        lhs.Token.CharPositionInLine + " Internal error. No such operation '" + op + "'");
                    symTable = null;
                    break;
            }

            return symTable;
        }

        public ISymbolType LogBop(LSLAst lhs, LSLAst rhs)
        {
            if (lhs.evalType != INT || rhs.evalType != INT)
            {
                _listener.Error("line " + lhs.Token.Line + ":" +
                    lhs.Token.CharPositionInLine + " Type mismatch, logical operators || and && require integer arguments");
            }

            return INT;
        }

        public bool HaveSameTypes(LSLAst lhs, LSLAst rhs)
        {
            if (lhs.evalType == null || rhs.evalType == null)
            {
                _listener.Error("line " + lhs.Token.Line + ":" +
                    lhs.Token.CharPositionInLine + "Can not compare types, at least one type is unknown " + lhs.Text + ", " + rhs.Text);

                return false;
            }

            if (lhs.evalType == rhs.evalType ||
                (lhs.promoteToType == rhs.promoteToType && lhs.promoteToType != null && rhs.promoteToType != null) ||
                lhs.evalType == rhs.promoteToType ||
                rhs.evalType == lhs.promoteToType)
            {
                return true;
            }

            return false;
        }

        public ISymbolType RelOp(LSLAst lhs, LSLAst rhs)
        {
            if (HasUnknownTypes(lhs, rhs))
            {
                return VOID;
            }

            //relational operations must be between the same types
            int tlhs = lhs.evalType.TypeIndex; // promote right to left type?
            int trhs = rhs.evalType.TypeIndex;

            rhs.promoteToType = promoteFromTo[trhs, tlhs];
            lhs.promoteToType = promoteFromTo[tlhs, trhs];

            if (!HaveSameTypes(lhs, rhs))
            {
                _listener.Error("line " + lhs.Token.Line + ":" +
                    lhs.Token.CharPositionInLine + " Type mismatch, relational operators require arguments of the same type");
            }

            //strings can not be LT/GT compared in LSL
            if (lhs.evalType == STRING || rhs.evalType == STRING)
            {
                _listener.Error("line " + lhs.Token.Line + ":" +
                    lhs.Token.CharPositionInLine + " Type mismatch, strings can not be compared with < or >");
            }

            return INT;
        }

        public ISymbolType EqOp(LSLAst lhs, LSLAst rhs)
        {
            if (HasUnknownTypes(lhs, rhs))
            {
                return VOID;
            }

            //equality operations must be between the same types
            int tlhs = lhs.evalType.TypeIndex; // promote right to left type?
            int trhs = rhs.evalType.TypeIndex;

            rhs.promoteToType = promoteFromTo[trhs, tlhs];
            lhs.promoteToType = promoteFromTo[tlhs, trhs];

            if (!HaveSameTypes(lhs, rhs))
            {
                _listener.Error("line " + lhs.Token.Line + ":" +
                    lhs.Token.CharPositionInLine + " Type mismatch, equality operators == and != require arguments of the same type");
            }

            return INT;
        }

        public ISymbolType BitOp(LSLAst lhs, LSLAst rhs)
        {
            if (HasUnknownTypes(lhs, rhs))
            {
                return INT;
            }

            if (lhs.evalType != INT || rhs.evalType != INT)
            {
                _listener.Error("line " + lhs.Token.Line + ":" +
                    lhs.Token.CharPositionInLine + " Type mismatch, bitwise operators require arguments of integral type");
            }

            return INT;
        }

        public ISymbolType Uminus(LSLAst a)
        {
            if (a.evalType == null)
            {
                _listener.Error(
                    "line " + a.Token.Line + ":" + a.Token.CharPositionInLine +
                    " Unknown type in expression -" + a.Text
                );

                return VOID;
            }

            //only integer, float, vector and rotations can be negated
            if (! TypeIsIn(a.evalType, new ISymbolType[] { INT, FLOAT, VECTOR, ROTATION }))
            {
                _listener.Error(
                    "line " + a.Token.Line + ":" + a.Token.CharPositionInLine +
                    " Unary minus (-N) not valid for type " + a.evalType.Name
                );
            }

            return a.evalType;
        }

        public ISymbolType UBoolNot(LSLAst a)
        {
            if (a.evalType == null)
            {
                _listener.Error(
                    "line " + a.Token.Line + ":" + a.Token.CharPositionInLine +
                    " Unknown type in expression !" + a.Text
                );

                return VOID;
            }

            if (a.evalType != INT)
            {
                _listener.Error(
                    "line " + a.Token.Line + ":" + a.Token.CharPositionInLine +
                    " Boolean not (!) not valid for type " + a.evalType.Name
                );

                return VOID;
            }

            return INT;
        }

        public ISymbolType PreInc(LSLAst a)
        {
            if (a.evalType == null)
            {
                _listener.Error(
                    "line " + a.Token.Line + ":" + a.Token.CharPositionInLine +
                    " (++) Unknown type in expression " + a.Text
                );

                return VOID;
            }

            if (! IsAssignable(a))
            {
                return VOID;
            }

            if (!TypeIsIn(a.evalType, new ISymbolType[] { INT, FLOAT }))
            {
                _listener.Error(
                    "line " + a.Token.Line + ":" + a.Token.CharPositionInLine +
                    " ++ not valid for type " + a.evalType.Name
                );

                return VOID;
            }

            return a.evalType;
        }

        public ISymbolType PreDec(LSLAst a)
        {
            if (a.evalType == null)
            {
                _listener.Error(
                    "line " + a.Token.Line + ":" + a.Token.CharPositionInLine +
                    " (--) Unknown type in expression " + a.Text
                );

                return VOID;
            }

            if (!IsAssignable(a))
            {
                return VOID;
            }

            if (!TypeIsIn(a.evalType, new ISymbolType[] { INT, FLOAT }))
            {
                _listener.Error(
                    "line " + a.Token.Line + ":" + a.Token.CharPositionInLine +
                    " -- not valid for type " + a.evalType.Name
                );

                return VOID;
            }

            return a.evalType;
        }

        public ISymbolType PostInc(LSLAst a)
        {
            return PreInc(a);
        }

        public ISymbolType PostDec(LSLAst a)
        {
            return PreDec(a);
        }

        public ISymbolType UBitNot(LSLAst a)
        {
            if (a.evalType == null)
            {
                _listener.Error(
                    "line " + a.Token.Line + ":" + a.Token.CharPositionInLine +
                    " Unknown type in expression !" + a.Text
                );

                return VOID;
            }

            if (a.evalType != INT)
            {
                _listener.Error(
                    "line " + a.Token.Line + ":" + a.Token.CharPositionInLine +
                    " Bitwise not (~) not valid for type " + a.evalType.Name
                );

                return VOID;
            }

            return INT;
        }

        private bool TypeIsIn(ISymbolType searchSymbol, ISymbolType[] goodSymbols)
        {
            return goodSymbols.Contains<ISymbolType>(searchSymbol);
        }

        public ISymbolType Unot(LSLAst a)
        {
            return INT;
        }

        public ISymbolType SubScript(LSLAst id, LSLAst subScript)
        {
            //the lhs must be an ID and it must be of vector type or rotation type
            //and it must be the letters x, y, z, or s for rotation
            if (id.Type != LSLParser.ID)
            {
                _listener.Error(
                    "line " + id.Token.Line + ":" + id.Token.CharPositionInLine +
                    " Use of subscript ." + subScript.Text + " requires a vector or rotation variable "
                );

                return VOID;
            }

            if (!TypeIsIn(id.evalType, new ISymbolType[] { VECTOR, ROTATION }))
            {
                _listener.Error(
                    "line " + id.Token.Line + ":" + id.Token.CharPositionInLine +
                    " Use of subscript ." + subScript.Text + " requires a vector or rotation variable"
                );

                return VOID;
            }

            if (id.evalType == VECTOR)
            {
                if (subScript.Text != "x" &&
                    subScript.Text != "y" &&
                    subScript.Text != "z")
                {
                    _listener.Error(
                        "line " + id.Token.Line + ":" + id.Token.CharPositionInLine +
                        " Invalid subscript ." + subScript.Text
                    );

                    return VOID;
                }
            }

            if (id.evalType == ROTATION)
            {
                if (subScript.Text != "x" &&
                    subScript.Text != "y" &&
                    subScript.Text != "z" &&
                    subScript.Text != "s")
                {
                    _listener.Error(
                        "line " + id.Token.Line + ":" + id.Token.CharPositionInLine +
                        " Invalid subscript ." + subScript.Text
                    );

                    return VOID;
                }
            }

            return FLOAT;
        }

        struct ParmAndExpected
        {
            public LSLAst Parm;
            public Symbol Expected;
        }

        public ISymbolType MethodCall(LSLAst id, IList<LSLAst> args)
        {
            Symbol s = id.scope.Resolve(id.Text + "()");

            if (s == null)
            {
                _listener.Error(
                    "line " + id.Token.Line + ":" + id.Token.CharPositionInLine +
                    " Call to undefined function " + id.Text + "()"
                );

                return VOID;
            }

            MethodSymbol ms = (MethodSymbol)s;
            id.symbol = ms;

            //check the call parameters
            if (!AllTypesAreResolved(args))
            {
                _listener.Error(
                    "line " + id.Token.Line + ":" + id.Token.CharPositionInLine +
                    " Method call contains expression(s) of unknown type"
                );

                return ms.Type;
            }

            //check param count and type
            if (args.Count != ms.Members.Count)
            {
                _listener.Error(
                    "line " + id.Token.Line + ":" + id.Token.CharPositionInLine +
                    " Function " + id.Text + "() takes " + Convert.ToString(ms.Members.Count) +
                    " parameters, " + args.Count + " given"
                );

                return ms.Type;
            }


            List<ParmAndExpected> erroredParms = new List<ParmAndExpected>();
            IEnumerator<Symbol> correctParms = ms.Members.GetEnumerator();
            foreach (LSLAst parm in args)
            {
                correctParms.MoveNext();

                //promote if necessary
                parm.promoteToType = promoteFromTo[parm.evalType.TypeIndex, correctParms.Current.Type.TypeIndex];

                if (! CanAssignTo(parm.evalType, correctParms.Current.Type, parm.promoteToType))
                {
                    erroredParms.Add(new ParmAndExpected { Parm = parm, Expected = correctParms.Current });
                }
            }

            //one of the parameter types is incompatible
            if (erroredParms.Count > 0)
            {
                _listener.Error("line " + id.Token.Line + ":" + id.Token.CharPositionInLine +
                    " In call of function " + id.Text + "(): ");

                foreach (ParmAndExpected error in erroredParms)
                {
                    _listener.Error(
                        "line " + error.Parm.Token.Line + ":" + error.Parm.Token.CharPositionInLine +
                        " Parameter " + error.Expected.Name + ", expected " + error.Expected.Type.Name +
                        " got " + error.Parm.evalType.Name
                    );
                }
            }

            return ms.Type;
        }

        public void CheckEvt(IScope currScope, LSLAst id)
        {   
            EventSymbol evt = (EventSymbol)currScope.Resolve(id.Text + "()");

            Debug.Assert(evt != null);

            //try to find this event in our table
            if (!_supportedEvents.HasEventByName(id.Text))
            {
                _listener.Error("line " + id.Token.Line + ":" +
                    id.Token.CharPositionInLine + " No event evailable with name '" + id.Text + "'");

                return;
            }

            //also try to resolve the arguments
            List<VarType> passedEvtArgs = evt.ExtractArgumentTypes();
            if (!_supportedEvents.HasEventBySig(id.Text, VarType.Void, passedEvtArgs))
            {
                string paramList = this.FormatParamTypeList(_supportedEvents.GetArguments(id.Text));
                _listener.Error("line " + id.Token.Line + ":" +
                    id.Token.CharPositionInLine + " Incorrect parameters for event " + id.Text +
                    paramList);
            }
        }

        private string FormatParamTypeList(IEnumerable<VarType> args)
        {
            StringBuilder listBuilder = new StringBuilder();
            listBuilder.Append("(");

            bool first = true;
            foreach (VarType type in args)
            {
                if (first) first = false;
                else listBuilder.Append(",");

                listBuilder.Append(indexToType[(int)type].Name);
            }

            listBuilder.Append(")");
            return listBuilder.ToString();
        }

        public void Define(StateSymbol stateSym, IScope currentScope)
        {
            if (stateSym.Name != "default(*)" && _globals.Resolve("default(*)") == null)
            {
                _listener.Error(
                    "line " + stateSym.Def.Line + ":" + stateSym.Def.CharPositionInLine + " State '"
                        + stateSym.Name + "' cannot be defined yet. Default state must be defined first."
                 );
            }

            this.Define((Symbol)stateSym, currentScope);
        }

        /// <summary>
        /// Tries to find a matching symbol in the function parameter scope
        /// </summary>
        /// <param name="sym"></param>
        /// <param name="localScope"></param>
        /// <returns></returns>
        private Symbol FindSymbolInMethodScope(Symbol sym, IScope localScope)
        {
            IScope searchScope = localScope;
            while ((searchScope = searchScope.EnclosingScope) != null)
            {
                MethodSymbol methScope = searchScope as MethodSymbol;
                if (methScope != null)
                {
                    if (methScope.IsDefinedLocally(sym.Name))
                    {
                        return methScope.Resolve(sym.Name);
                    }
                }

                EventSymbol eventScope = searchScope as EventSymbol;
                if (eventScope != null)
                {
                    if (eventScope.IsDefinedLocally(sym.Name))
                    {
                        return eventScope.Resolve(sym.Name);
                    }
                }
            }

            return null;
        }

        public bool TestForDefineErrors(Symbol sym, IScope currentScope)
        {
            string location = string.Empty;
            if (sym.Def != null)
                location = "line " + sym.Def.Line + ":" + sym.Def.CharPositionInLine + " ";

            if (currentScope.IsDefinedLocally(sym.Name))
            {
                _listener.Error(location + "Symbol '"
                            + sym.Name + "' already defined"
                    );
                return true;
            }

            if (currentScope.ScopeName == "local")
            {
                //if we're in a local scope, travel up until we hit a function def scope
                //or an event def scope and make sure we're not shadowing any parameters
                //if we are, issue a warning

                if (FindSymbolInMethodScope(sym, currentScope) != null)
                {
                    _listener.Info(location + "Symbol '"
                                + sym.Name + "' shadows parameter of the same name"
                         );
                }
            }

            return false;
        }

        public void Define(Symbol sym, IScope currentScope)
        {
            if (TestForDefineErrors(sym, currentScope))
            {
                return;
            }

            currentScope.Define(sym);
        }

        public void Define(VariableSymbol varSym, IScope currentScope)
        {
            if (TestForDefineErrors(varSym, currentScope))
            {
                return;
            }

            currentScope.Define(varSym);
        }

        public bool CanAssignTo(ISymbolType valueType, ISymbolType destType, ISymbolType promotion)
        {
            // either types are same or value was successfully promoted
            return valueType == destType || promotion == destType;
        }

        public bool IsAssignable(LSLAst lhs)
        {
            //lhs will either be an id direct or an EXPR with an ID child
            if (lhs.Type == LSLParser.ID)
            {
                return true;
            }

            if (lhs.Type == LSLParser.EXPR &&
                lhs.ChildCount == 1 &&
                lhs.Children[0].Type == LSLParser.ID)
            {
                return true;
            }

            if (lhs.Type == LSLParser.SUBSCRIPT)
            {
                return true;
            }

            
            _listener.Error(
                "line " + lhs.Line + ":" + lhs.CharPositionInLine + " '" + 
                lhs.Text + "' is not assignable"
            );

            return false;
        }

        public ISymbolType Assign(string type, LSLAst lhs, LSLAst rhs)
        {
            if (HasUnknownTypes(lhs, rhs))
            {
                return VOID;
            }

            //the left hand side needs to be assignable
            if (!IsAssignable(lhs))
            {
                return VOID;
            }

            int tlhs = lhs.evalType.TypeIndex; // promote right to left type?
            int trhs = rhs.evalType.TypeIndex;

            if (type == "=")
            {
                return StdAssign(lhs, rhs, tlhs, trhs);
            }

            ISymbolType[,] opTable = FindOperationTable(lhs, type);
            if (opTable == null)
            {
                return VOID;
            }

            ISymbolType symType = opTable[lhs.evalType.TypeIndex, rhs.evalType.TypeIndex];
            if (symType == VOID)
            {
                if (lhs.Type == LSLParser.EXPR)
                {
                    lhs = (LSLAst)lhs.Children[0];
                }

                _listener.Error("line " + lhs.Token.Line + ":" +
                    lhs.Token.CharPositionInLine + " '" + type + "' is not a valid operation between " +
                    lhs.evalType.Name + " and " + rhs.evalType.Name);
                return VOID;
            }

            return symType;
        }

        private ISymbolType StdAssign(LSLAst lhs, LSLAst rhs, int tlhs, int trhs)
        {
            rhs.promoteToType = promoteFromTo[trhs, tlhs];
            if (!CanAssignTo(rhs.evalType, lhs.evalType, rhs.promoteToType))
            {
                _listener.Error(
                    "line " + lhs.Line + ":" + lhs.CharPositionInLine + " " + text(lhs) + " and " +
                    text(rhs) + " have incompatible types in " +
                    text((LSLAst)lhs.Parent)
                );

                return VOID;
            }

            return lhs.evalType;
        }

        public void DeclInit(LSLAst varName, LSLAst initExpr)
        {
            int tlhs = varName.symbol.Type.TypeIndex; // promote right to left type?
            varName.evalType = varName.symbol.Type; 
            int trhs = initExpr.evalType.TypeIndex;

            initExpr.promoteToType = promoteFromTo[trhs, tlhs];
            if (!CanAssignTo(initExpr.evalType, varName.evalType, initExpr.promoteToType))
            {
                _listener.Error(
                    "line " + varName.Line + ":" + varName.CharPositionInLine + " " + text(varName) + " and " +
                    text(initExpr) + " have incompatible types in " +
                    text((LSLAst)initExpr.Parent)
                );
            }
        }

        public bool CanCast(int from, int to)
        {
            if (castFromTo[from, to] != VOID)
            {
                return true;
            }

            return false;
        }

        public ISymbolType TypeCast(LSLAst expr, LSLAst type)
        {
            if (HasUnknownType(expr))
            {
                return VOID;
            }

            int texpr = expr.evalType.TypeIndex;
            
            //type must be a builtin type
            ISymbolType toType = (ISymbolType)_globals.Resolve(type.Text);
            int ttype = toType.TypeIndex;

            if (!CanCast(texpr, ttype))
            {
                _listener.Error(
                    "line " + type.Line + ":" + type.CharPositionInLine + " Can not cast from " 
                    + expr.evalType.Name + " to " + type.Text
                );
            }

            return toType;
        }

        public string text(LSLAst t)
        {
            string ts = "";
            //if (t.evalType != null) ts = ":<" + t.evalType + ">";
            return _tokens.ToString(t.TokenStartIndex,
                                   t.TokenStopIndex) + ts;
        }

        public bool AllTypesAreResolved(IEnumerable<LSLAst> exprs)
        {
            foreach (LSLAst expr in exprs)
            {
                if (expr.evalType == null)
                {
                    return false;
                }
            }

            return true;
        }

        public bool CheckVectorLiteral(LSLAst e1, LSLAst e2, LSLAst e3)
        {
            if (! AllTypesAreResolved(new LSLAst[] {e1, e2, e3}))
            {
                _listener.Error(
                    "line " + e1.Token.Line + ":" + e1.Token.CharPositionInLine +
                    " Vector contains expression(s) of unknown type"
                );

                return false;
            }

            //try type promotion first
            e1.promoteToType = promoteFromTo[e1.evalType.TypeIndex, FLOAT.TypeIndex];
            e2.promoteToType = promoteFromTo[e2.evalType.TypeIndex, FLOAT.TypeIndex];
            e3.promoteToType = promoteFromTo[e3.evalType.TypeIndex, FLOAT.TypeIndex];

            if (CanAssignTo(e1.evalType, FLOAT, e1.promoteToType) &&
                CanAssignTo(e2.evalType, FLOAT, e2.promoteToType) &&
                CanAssignTo(e3.evalType, FLOAT, e3.promoteToType))
            {
                return true;
            }

            _listener.Error(
                    "line " + e1.Token.Line + ":" + e1.Token.CharPositionInLine + 
                    " Vector components must be float or implicitly convertable to float "
                );

            return false;
        }

        public bool CheckRotationLiteral(LSLAst e1, LSLAst e2, LSLAst e3, LSLAst e4)
        {
            if (! AllTypesAreResolved(new LSLAst[] {e1, e2, e3, e4}))
            {
                _listener.Error(
                    "line " + e1.Token.Line + ":" + e1.Token.CharPositionInLine +
                    " Rotation contains expression(s) of unknown type"
                );

                return false;
            }

            //try type promotion first
            e1.promoteToType = promoteFromTo[e1.evalType.TypeIndex, FLOAT.TypeIndex];
            e2.promoteToType = promoteFromTo[e2.evalType.TypeIndex, FLOAT.TypeIndex];
            e3.promoteToType = promoteFromTo[e3.evalType.TypeIndex, FLOAT.TypeIndex];
            e4.promoteToType = promoteFromTo[e4.evalType.TypeIndex, FLOAT.TypeIndex];

            if (CanAssignTo(e1.evalType, FLOAT, e1.promoteToType) &&
                CanAssignTo(e2.evalType, FLOAT, e2.promoteToType) &&
                CanAssignTo(e3.evalType, FLOAT, e3.promoteToType) &&
                CanAssignTo(e4.evalType, FLOAT, e4.promoteToType))
            {
                return true;
            }

            _listener.Error(
                    "line " + e1.Token.Line + ":" + e1.Token.CharPositionInLine +
                    " Rotation components must be float or implicitly convertable to float "
                );

            return false;
        }

        public void CheckReturn(LSLAst retStmt, LSLAst retExpr)
        {
            //return must match function type
            if (retExpr == null)
            {
                if (retStmt.symbol.Type == VOID)
                {
                    //void return type with void result
                    return;
                }
                else
                {
                    _listener.Error(
                        "line " + retStmt.Token.Line + ":" + retStmt.Token.CharPositionInLine +
                        " Function must return a value"
                    );

                    return;
                }
                
            }

            if (HasUnknownType(retExpr))
            {
                return;
            }

            //check promotions and return
            retExpr.promoteToType = promoteFromTo[retExpr.evalType.TypeIndex, retStmt.symbol.Type.TypeIndex];
            if (! CanAssignTo(retExpr.evalType, retStmt.symbol.Type, retExpr.promoteToType))
            {
                _listener.Error(
                    "line " + retStmt.Token.Line + ":" + retStmt.Token.CharPositionInLine +
                    " Invalid return type " + retExpr.evalType.Name + ", expecting " + retStmt.symbol.Type.Name
                );
            }

        }

        public void CheckStateChange(LSLAst chgNode, LSLAst destID)
        {
            string stateName = destID != null ? destID.Text + "(*)" : "default(*)";
            StateSymbol state = _globals.Resolve(stateName) as StateSymbol;
            if (state == null)
            {
                _listener.Error(
                    "line " + chgNode.Token.Line + ":" + chgNode.Token.CharPositionInLine +
                    " Undefined state " + stateName
                );
            }
        }

        public void VerifyDefaultState()
        {
            StateSymbol state = _globals.Resolve("default(*)") as StateSymbol;
            if (state == null)
            {
                _listener.Error(
                    "line: 0:0" + " No default state defined"
                );
            }
        }

        public void CheckLogicalExpr(LSLAst expr)
        {
            if (expr.evalType != INT)
            {
                _listener.Error("line " + expr.Token.Line + ":" +
                    expr.Token.CharPositionInLine + " Logical expressions require integer type");
            }
        }

        public void CheckJump(LSLAst jumpStmt, LSLAst id)
        {
            LabelSymbol label = jumpStmt.scope.Resolve("@" + id.Text) as LabelSymbol;
            if (label == null)
            {
                _listener.Error("line " + id.Token.Line + ":" +
                    id.Token.CharPositionInLine + " Undefined label " + id.Text);
            }
            else
            {
                id.symbol = label;
            }
        }

        public ISymbolType CheckListLiteral(LSLAst start, IList<LSLAst> args)
        {
            foreach (LSLAst node in args)
            {
                if (node.evalType == LIST)
                {
                    _listener.Error("line " + start.Token.Line + ":" +
                        start.Token.CharPositionInLine + " A list can not contain another list");
                }
            }

            return LIST;
        }
    }
}
