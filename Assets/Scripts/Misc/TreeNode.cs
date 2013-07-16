/*!
  \brief This class is a simple binary tree
  \author Pierre COLLET
  \mail pierre.collet91@gmail.com
 */
public class TreeNode<T>
{
  private T             _data;          //!< Data that the node contains
  private TreeNode<T>       _left;      //!< left node
  private TreeNode<T>       _right;     //!< right node

  //! Default constructor
  public TreeNode(T data, TreeNode<T> left = null, TreeNode<T> right = null)
  {
    _data = data;
    _left = left;
    _right = right;
  }

  public TreeNode<T>       getLeftNode() {return _left;}
  public TreeNode<T>       getRightNode() {return _right;}
  public T                 getData() {return _data;}

  public void              setLeftNode(TreeNode<T> node) {_left = node;}
  public void              setRightNode(TreeNode<T> node) {_right = node;}
  public void              setData(T node) {_data = node;}
}
