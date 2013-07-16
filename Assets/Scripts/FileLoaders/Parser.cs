using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using System.IO;
using System.Text;

public class PromoterLexer
{
  public enum eToken
  {
    OP_OR,
    OP_AND,
    LPAR,
    RPAR,
    OP_NOT,
    LHOOK,
    RHOOK,
    COMMA,
    NUM,
    CHAR,
    END
  }

  public struct Token
  {
    public Token(eToken t, string c)
    {
      this.token = t;
      this.c = c;
    }
    public eToken       token;
    public string       c;
  }

  static private Token[] _tokenTab = new Token[]
    {
      new Token(eToken.OP_OR, "|"),
      new Token(eToken.OP_AND, "*"),
      new Token(eToken.LPAR, "("),
      new Token(eToken.RPAR, ")"),
      new Token(eToken.OP_NOT, "!"),
      new Token(eToken.LHOOK, "["),
      new Token(eToken.RHOOK, "]"),
      new Token(eToken.COMMA, ","),
      new Token(eToken.NUM, "0123456789."),
    };

  
  public PromoterLexer.Token getToken(char c)
  {
    foreach (Token i in _tokenTab)
      {
        if (i.c.IndexOf(c) != -1)
          return new Token(i.token, new string(c, 1));
      }
    return new Token(eToken.CHAR, new string(c, 1));
  }

  public LinkedList<Token> lex(string str)
  {
    LinkedList<Token> tokList = new LinkedList<Token>();

    foreach (char c in str)
      tokList.AddLast(getToken(c));
    tokList.AddLast(new Token (eToken.END, ""));
    return tokList;
  }

// ==================== Debug ========================
  
  public void PPTokenList(LinkedList<Token> list)
  {
    int i = 0;
    string tokStr = "UNKNOWN_TOKEN =(";

    foreach (Token t in list)
      {
        switch (t.token)
          {
          case eToken.OP_OR:
            tokStr = "OP_OR";
            break;
          case eToken.OP_AND:
            tokStr = "OP_AND";
            break;
          case eToken.LPAR:
            tokStr = "LPAR";
            break;
          case eToken.RPAR:
            tokStr = "RPAR";
            break;
          case eToken.OP_NOT:
            tokStr = "OP_NOT";
            break;
          case eToken.LHOOK:
            tokStr = "LHOOK";
            break;
          case eToken.RHOOK:
            tokStr = "RHOOK";
            break;
          case eToken.NUM:
            tokStr = "NUM";
            break;
          case eToken.CHAR:
            tokStr = "CHAR";
            break;
          case eToken.END:
            tokStr = "END";
            break;
          }
        Debug.Log("Token " + i + " : " + t.c + " -> " + tokStr);
        i++;
      }
  }
}

public class PromoterNodeData
{
  public PromoterNodeData(PromoterParser.eNodeType t = default(PromoterParser.eNodeType), string v = default(string))
  {
    token = t;
    value = v;
  }
  public PromoterParser.eNodeType   token  {get; set;}
  public string         value  {get; set;}
}

public class PromoterParser
{
  public enum eNodeType
  {
    OR,
    AND,
    NOT,
    CONSTANT,
    WORD,
    NUM,
    BOOL
  }


  LinkedList<PromoterLexer.Token>     _restoreList;
  int                   _restoreStatus;

  public PromoterParser()
  {
    _restoreStatus = 0;
    _restoreList = new LinkedList<PromoterLexer.Token>();
  }



  public void           popToken(LinkedList<PromoterLexer.Token> list)
  {
    PromoterLexer.Token tok = list.First();
    list.RemoveFirst();
    _restoreList.AddFirst(tok);
    _restoreStatus += 1;
  }

  public void           restoreToken(LinkedList<PromoterLexer.Token> list)
  {
    PromoterLexer.Token tok = _restoreList.First();
    _restoreList.RemoveFirst();
    list.AddFirst(tok);
    _restoreStatus -= 1;
  }

  public void           restoreListState(LinkedList<PromoterLexer.Token> list, int state)
  {
    while (_restoreStatus > state)
      restoreToken(list);
  }

  public int            getRestoreListStatus()
  {
    return _restoreStatus;
  }

  public TreeNode<PromoterNodeData>     ParseFormula(LinkedList<PromoterLexer.Token> tokenList)
  {
    return ParseORExpr(tokenList);
  }
  public TreeNode<PromoterNodeData>     ParseORExpr(LinkedList<PromoterLexer.Token> tokenList)
  {
    PromoterNodeData data = new PromoterNodeData();
    TreeNode<PromoterNodeData> left = null;
    TreeNode<PromoterNodeData> right = null;
    int restoreStatus;

    restoreStatus = getRestoreListStatus();
    if ((left = ParseAndExpr(tokenList)) == null)
      {
        restoreListState(tokenList, restoreStatus);
        return null;
      }
    if (tokenList.First().token == PromoterLexer.eToken.OP_OR)
      {
        PromoterLexer.Token tok = tokenList.First();
        popToken(tokenList);
        if ((right = ParseORExpr(tokenList)) == null)
          {
            restoreListState(tokenList, restoreStatus);
            Debug.Log("Syntax error : bad OR expr");
            return null;
          }
        data.token = PromoterParser.eNodeType.OR;
        data.value = tok.c;
        return new TreeNode<PromoterNodeData>(data, left, right);
      }
    else
      return left;
  }

  public TreeNode<PromoterNodeData>     ParseAndExpr(LinkedList<PromoterLexer.Token> tokenList)
  {
    PromoterNodeData data = new PromoterNodeData();
    TreeNode<PromoterNodeData> left = null;
    TreeNode<PromoterNodeData> right = null;
    int restoreStatus;

    restoreStatus = getRestoreListStatus();
    if ((left = ParseParExpr(tokenList)) == null)
      {
        restoreListState(tokenList, restoreStatus);
        return null;
      }
    if (tokenList.First().token == PromoterLexer.eToken.OP_AND)
      {
        PromoterLexer.Token tok = tokenList.First();
        popToken(tokenList);
        if ((right = ParseAndExpr(tokenList)) == null)
          {
            restoreListState(tokenList, restoreStatus);
            Debug.Log("Syntax error : bad AND expr");
            return null;
          }
        data.token = PromoterParser.eNodeType.AND;
        data.value = tok.c;
        return new TreeNode<PromoterNodeData>(data, left, right);
      }
    else
      return left;
  }

  public TreeNode<PromoterNodeData>     ParseParExpr(LinkedList<PromoterLexer.Token> tokenList)
  {
    int restoreStatus;
    TreeNode<PromoterNodeData> node = null;        

    restoreStatus = getRestoreListStatus();
    if (tokenList.First().token == PromoterLexer.eToken.LPAR)
      {
        popToken(tokenList);
        if ((node = ParseORExpr(tokenList)) == null)
          {
            restoreListState(tokenList, restoreStatus);
            Debug.Log("Syntax error : bad ParExpr");
            return null;
          }
        if (tokenList.First().token != PromoterLexer.eToken.RPAR)
          {
            restoreListState(tokenList, restoreStatus);
            Debug.Log("Syntax error : bad ParExpr : missing closing parenthis");
            return null;
          }
        popToken(tokenList);
        return node;
      }
    if ((node = ParseNotExpr(tokenList)) == null)
      {
        restoreListState(tokenList, restoreStatus);
        return null;
      }
    return node;
  }


  public TreeNode<PromoterNodeData>     ParseNotExpr(LinkedList<PromoterLexer.Token> tokenList)
  {
    PromoterNodeData data = new PromoterNodeData();
    TreeNode<PromoterNodeData> node = new TreeNode<PromoterNodeData>(data);
    int restoreStatus;

    restoreStatus = getRestoreListStatus();
    if (tokenList.First().token == PromoterLexer.eToken.OP_NOT)
      {
        PromoterLexer.Token tok = tokenList.First();
        popToken(tokenList);
        if ((node = ParseOperandExpr(tokenList)) == null)
          {
            restoreListState(tokenList, restoreStatus);
            Debug.Log("Syntax Error: Bad Not Expr");
            return null;
          }
        data.token = PromoterParser.eNodeType.NOT;
        data.value = tok.c;
        return new TreeNode<PromoterNodeData>(data, node);
      }
    return ParseOperandExpr(tokenList);
  }

  public TreeNode<PromoterNodeData>     ParseOperandExpr(LinkedList<PromoterLexer.Token> tokenList)
  {
    TreeNode<PromoterNodeData> left;
    TreeNode<PromoterNodeData> right;
    PromoterNodeData    data = new PromoterNodeData();
    int restoreStatus;

    restoreStatus = getRestoreListStatus();

    if ((right = ParseBool(tokenList)) != null)
      {
        data.token = PromoterParser.eNodeType.CONSTANT;
        data.value = "C";
        return new TreeNode<PromoterNodeData>(data, null, right);
      }
    if ((left = ParseConstants(tokenList)) == null)
      {
        restoreListState(tokenList, restoreStatus);
        Debug.Log("Syntax error : No constant defined");
        return null;
      }
    if ((right = ParseWord(tokenList)) == null)
      {
        restoreListState(tokenList, restoreStatus);
        Debug.Log("Syntax error : Need name for operand");
        return null; 
      }
    data.token = PromoterParser.eNodeType.CONSTANT;
    data.value = "C";
    return new TreeNode<PromoterNodeData>(data, left, right);
  }

  public TreeNode<PromoterNodeData>     ParseBool(LinkedList<PromoterLexer.Token> tokenList)
  {
    PromoterNodeData data = new PromoterNodeData();
    int restoreStatus;
    string value = "";

    restoreStatus = getRestoreListStatus();
    if (tokenList.First().token != PromoterLexer.eToken.CHAR)
      {
        restoreListState(tokenList, restoreStatus);
        Debug.Log("Syntax error : A word need to begin with a Character.");
        return null;
      }
    value += tokenList.First().c;
    popToken(tokenList);

    if (value == "T" || value == "F")
      data.token = PromoterParser.eNodeType.BOOL;
    else
      {
        restoreListState(tokenList, restoreStatus);
        return null;        
      }
    data.value = value;
    return new TreeNode<PromoterNodeData>(data);
  }

  public TreeNode<PromoterNodeData>     ParseConstantsNumList(LinkedList<PromoterLexer.Token> tokenList)
  {
    int restoreStatus;
    TreeNode<PromoterNodeData> nodeK;
    TreeNode<PromoterNodeData> nodeN;

    restoreStatus = getRestoreListStatus();
    if ((nodeK = ParseNumber(tokenList)) == null)
      {
        Debug.Log("Synthax error : Missing Beta parameter constant number list in formula");
        restoreListState(tokenList, restoreStatus);
        return null;
      }

    if (tokenList.First().token != PromoterLexer.eToken.COMMA)
      {
        restoreListState(tokenList, restoreStatus);
        Debug.Log("Syntax error : Need a ',' character to separate constants numbers");
        return null;
      }
    popToken(tokenList);
    if ((nodeN = ParseNumber(tokenList)) == null)
      {
        Debug.Log("Synthax error : Missing \"n\" parameter constant number list in formula");
        restoreListState(tokenList, restoreStatus);
        return null;
      }
    nodeK.setLeftNode(nodeN);
    return nodeK;
  }

  public TreeNode<PromoterNodeData>     ParseConstants(LinkedList<PromoterLexer.Token> tokenList)
  {
    TreeNode<PromoterNodeData> node;
    int restoreStatus;

    restoreStatus = getRestoreListStatus();
    if (tokenList.First().token != PromoterLexer.eToken.LHOOK)
      {
        Debug.Log("Syntax error : Need a '[' character to define constants");
        return null;
      }
    popToken(tokenList);
    if ((node = ParseConstantsNumList(tokenList)) == null)
      {
        restoreListState(tokenList, restoreStatus);
        return null;
      }
    if (tokenList.First().token != PromoterLexer.eToken.RHOOK)
      {
        restoreListState(tokenList, restoreStatus);
        Debug.Log("Syntax error : Need a ']' character to define constants");
        return null;
      }
    popToken(tokenList);
    return node;
  }

  public TreeNode<PromoterNodeData>     ParseWord(LinkedList<PromoterLexer.Token> tokenList)
  {
    PromoterNodeData data = new PromoterNodeData();
    int restoreStatus;
    string value = "";

    restoreStatus = getRestoreListStatus();
    if (tokenList.First().token != PromoterLexer.eToken.CHAR)
      {
        restoreListState(tokenList, restoreStatus);
        Debug.Log("Syntax error : A word need to begin with a Character.");        
        return null;
      }
    value += tokenList.First().c;
    popToken(tokenList);
    while (tokenList.First().token == PromoterLexer.eToken.CHAR || tokenList.First().token == PromoterLexer.eToken.NUM)
      {
        value += tokenList.First().c;
        popToken(tokenList);
      }
    data.token = PromoterParser.eNodeType.WORD;
    data.value = value;
    return new TreeNode<PromoterNodeData>(data);
  }

  public TreeNode<PromoterNodeData>     ParseNumber(LinkedList<PromoterLexer.Token> tokenList)
  {
    PromoterNodeData data = new PromoterNodeData();
    int restoreStatus;
    string value = "";

    restoreStatus = getRestoreListStatus();
    if (tokenList.First().token != PromoterLexer.eToken.NUM)
      {
        restoreListState(tokenList, restoreStatus);
        Debug.Log("Syntax error : A Number need to begin with a number.");
        return null;
      }
    value += tokenList.First().c;
    popToken(tokenList);
    while (tokenList.First().token == PromoterLexer.eToken.NUM)
      {
        value += tokenList.First().c;
        popToken(tokenList);
      }
    data.token = PromoterParser.eNodeType.WORD;
    data.value = value;
    return new TreeNode<PromoterNodeData>(data);
  }


  public TreeNode<PromoterNodeData> Parse(string str)
  {
    clear();
    PromoterLexer lex = new PromoterLexer();
    LinkedList<PromoterLexer.Token> tokenList = lex.lex(str);
//     lex.PPTokenList(tokenList);
    TreeNode<PromoterNodeData> tree = ParseFormula(tokenList);
    if (tokenList.First().token == PromoterLexer.eToken.END)
      return tree;
    Debug.Log("Parsing Error for expression " + str);
    return null;
  }

  public void clear()
  {
    _restoreList.Clear();
    _restoreStatus = 0;
  }



// ========================== Pretty Print Tree =================================

  public void  PPRecTree(TreeNode<PromoterNodeData> node, ref string str)
  {
    if (node != null)
      {
        if (node.getLeftNode() != null)
          {
            str += "\"" + node.getData().value + "\"" + "->" + "\"" +node.getLeftNode().getData().value + "\"\n";
            PPRecTree(node.getLeftNode(), ref str);            
          }
        if (node.getRightNode() != null)
          {
            str += "\"" + node.getData().value + "\"" + "->" + "\"" + node.getRightNode().getData().value + "\"\n";
            PPRecTree(node.getRightNode(), ref str);
          }
      }
  }

  public void PPTree(TreeNode<PromoterNodeData> tree)
  {
    string output;
    if (tree == null)
      Debug.Log("failed");
    output = "Digraph G {";
    PPRecTree(tree, ref output);
    output += "}";
//     string path = "graph.txt";

    // This text is added only once to the file. 
//     if (!File.Exists(path))
//       {
//         File.WriteAllText(path, output);
//       }
  }
}