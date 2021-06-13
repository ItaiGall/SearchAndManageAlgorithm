using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DSProject_ItaiG.ClassLibrary
{
    internal class BinarySearchTree<T> where T : IComparable<T>
    {
        Node root;

        public bool IsIncluded(T item, string mode, out T foundItem) //search function
        {
            Node tmp = root;

            if (root == null)
            {
                foundItem = item;
                return false;
            }
            while (tmp != null)
            {
                if (mode == "exact") //exact search
                {
                    if (item.CompareTo(tmp.info) > 0)
                        tmp = tmp.right;
                    else if (item.CompareTo(tmp.info) < 0)
                        tmp = tmp.left;
                    else if (item.CompareTo(tmp.info) == 0)
                    {
                        foundItem = tmp.info;
                        return true;
                    }
                }
                else //search for the optimal box
                {
                    foundItem = ApproxSearch(item, tmp);
                    if (foundItem == null)
                        return false;
                    return true;
                }
            }
            foundItem = item;
            return false;
        }

        public T ApproxSearch(T item, Node tmp)
        {
            if (tmp.right == null && tmp.left == null) //only one single node
            {
                if (item.CompareTo(tmp.info) <= 0)
                    return tmp.info;
                return default(T);
            }
            else if (item.CompareTo(tmp.info) < 0) //left of root
            {
                while (tmp.right != null || tmp.left != null) //forward iteration
                {
                    if (item.CompareTo(tmp.info) == 0)
                        return tmp.info;

                    if (item.CompareTo(tmp.info) < 0)
                    {
                        if (tmp.right != null && item.CompareTo(tmp.left.info) > 0)
                            tmp = tmp.right;
                        else
                            tmp = tmp.left;
                    }
                    else
                    {
                        tmp = tmp.right;
                    }
                }
                while (item.CompareTo(tmp.info) > 0) //backward iteration
                {
                    tmp = tmp.previous;
                }
                return tmp.info;
            }
            else //right of root
            {
                while (tmp.right != null || tmp.left != null) //forward iteration
                {
                    if (item.CompareTo(tmp.info) == 0)
                        return tmp.info;

                    if (item.CompareTo(tmp.info) < 0)
                        tmp = tmp.left;
                    else
                        tmp = tmp.right;
                }
                if (item.CompareTo(tmp.info) > 0 && item.CompareTo(tmp.previous.info) > 0) //item is beyond max val of BST
                    return default(T);
                else
                {
                    while (item.CompareTo(tmp.info) > 0) //backward iteration
                    {
                        tmp = tmp.previous;
                    }
                    return tmp.info;
                }
            }
        }

        public void Add(T val)
        {
            Node newNode = new Node(val);
            if (root == null)
            {
                root = newNode;
                return;
            }

            Node tmp = root;
            Node parent = null;
            while (tmp != null)
            {
                parent = tmp;
                if (val.CompareTo(tmp.info) < 0) //val < tmp.info
                    tmp = tmp.left;
                else
                    tmp = tmp.right;

            }
            if (val.CompareTo(parent.info) < 0) //add value as left node, val<parent.info
                parent.left = newNode;
            else
                parent.right = newNode;

            newNode.previous = parent;
        }

        public void Remove(T val)
        {
            RemoveNode(val, root);
        }
        private Node RemoveNode(T val, Node root)
        {
            if (root == null)
                return root;

            if (val.CompareTo(root.info) < 0)
                root.left = RemoveNode(val, root.left);
            else if (val.CompareTo(root.info) > 0)
                root.right = RemoveNode(val, root.right);
            else
            {
                if (root.left == null)
                    return root.right;
                else if (root.right == null)
                    return root.left;
                else
                    root.info = MinValue(root.right);

                root.right = RemoveNode(root.info, root.right);
            }

            return root;
        }

        private T MinValue(Node root)
        {
            T min = root.info;
            while (root.left != null)
            {
                min = root.left.info;
                root = root.left;
            }
            return min;
        }

        public void PrintInOrder(Action<T> action)
        {
            PrintInOrder(root, action);
        }

        private void PrintInOrder(Node currentRoot, Action<T> action)
        {
            if (currentRoot == null)
                return;
            PrintInOrder(currentRoot.left, action);
            action(currentRoot.info);
            Console.Write("\t");
            PrintInOrder(currentRoot.right, action);
        }

        public int CountLevels()
        {
            return CountLevels(root);
        }

        private int CountLevels(Node currentRoot)
        {
            if (currentRoot == null)
                return 0;
            int leftlevels = CountLevels(currentRoot.left);

            int rightlevels = CountLevels(currentRoot.right);

            return Math.Max(rightlevels, leftlevels) + 1;
        }

        internal class Node
        {
            public T info;
            public Node left;
            public Node right;
            public Node previous;

            public Node(T info)
            {
                this.info = info;
                left = right = previous = null;
            }
        }
    }
}
