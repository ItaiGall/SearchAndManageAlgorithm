using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;


namespace DSProject_ItaiG.ClassLibrary
{
    public class ClassManager
    {
        BinarySearchTree<BoxBase> BoxBaseTree;
        TimeManager<TimeData> QueueList;
        readonly static int MaxStock = 20;
        readonly static int ThresholdStock = 1;
        const int dueCheckMin = 5;
        const int periodCheckMin = 1;
        readonly Timer removeOldTimer;

        public ClassManager()
        {
            BoxBaseTree = new BinarySearchTree<BoxBase>();
            QueueList = new TimeManager<TimeData>();
            removeOldTimer = new Timer(RemoveFromStockAfterTimeOut,
                                   null, new TimeSpan(0, dueCheckMin, 0), new TimeSpan(0, periodCheckMin, 0));
        }

        public bool Restock(double xValue, double yValue, int stock, out bool IsBeyondThreshold)
        {
            BoxBase foundBaseX;
            BoxBase newX = new BoxBase(xValue);
            BoxHeight foundBaseY = null;
            BoxHeight newY = new BoxHeight(yValue, stock);
            IsBeyondThreshold = false;
            if (BoxBaseTree.IsIncluded(newX, "exact", out foundBaseX))
            {
                if (foundBaseX.BoxHeightTree.IsIncluded(newY, "exact", out foundBaseY))
                {
                    newY.timeNodeReference = QueueList.end;
                    foundBaseY.Stock += stock;
                    if (foundBaseY.Stock <= MaxStock)
                    {
                        if (foundBaseY.Stock <= ThresholdStock)
                        {
                            IsBeyondThreshold = true;
                            return true;
                        }
                        return true;
                    }
                    foundBaseY.Stock = MaxStock;
                    return true;
                }
                else
                {
                    foundBaseX.BoxHeightTree.Add(newY);
                    QueueList.EnQueue(new TimeData(foundBaseX, newY));
                    newY.timeNodeReference = QueueList.end;
                    if (newY.Stock <= MaxStock)
                    {
                        if (newY.Stock <= ThresholdStock)
                        {
                            IsBeyondThreshold = true;
                            return true;
                        }
                        return true;
                    }
                    newY.Stock = MaxStock;
                    return true;
                }
            }
            else
            {
                BoxBaseTree.Add(newX);
                newX.BoxHeightTree.Add(newY);
                QueueList.EnQueue(new TimeData(newX, newY));
                newY.timeNodeReference = QueueList.end;
                if (newY.Stock <= MaxStock)
                {
                    if (newY.Stock <= ThresholdStock)
                    {
                        IsBeyondThreshold = true;
                        return true;
                    }
                    return true;
                }
                newY.Stock = MaxStock;
                return true;
            }
        }

        public string ShowStock(double xValue, double yValue)
        {
            int levels = BoxBaseTree.CountLevels();
            if (levels > 0)
            {
                BoxBase foundBaseX;
                BoxHeight foundBaseY;
                if (BoxBaseTree.IsIncluded(new BoxBase(xValue), "exact", out foundBaseX)
                    && foundBaseX.BoxHeightTree.IsIncluded(new BoxHeight(yValue), "exact", out foundBaseY))
                {
                    string StandardMessage = $"\nThe requested box is in stock with {foundBaseY.Stock} pieces.";
                    if (foundBaseY.Stock > ThresholdStock)
                        return StandardMessage;
                    else
                        return StandardMessage + "\nThe added box is prone to be removed from the stock due to few pieces left.\n";
                }
                else
                {
                    return "\nThe requested box is currently NOT in stock.";
                }
            }
            else
            {
                return "\nThe stock is empty";
            }
        }

        public string FindOptimalBox(double xDesiredValue, double yDesiredValue)
        {
            int levels = BoxBaseTree.CountLevels();
            BoxBase foundDesiredBaseX;
            BoxHeight foundDesiredHeightY;

            if (levels > 0)
            {
                if (BoxBaseTree.IsIncluded(new BoxBase(xDesiredValue), "approximately", out foundDesiredBaseX)
                    && foundDesiredBaseX.BoxHeightTree.IsIncluded(new BoxHeight(yDesiredValue), "approximately", out foundDesiredHeightY))
                {
                    string StandardMessage = $"\nThere is a fitting box with a base of {foundDesiredBaseX.XValue} on {foundDesiredBaseX.XValue}" +
                        $" and a height of {foundDesiredHeightY.YValue}.\nThere are {foundDesiredHeightY.Stock} more boxes of this kind left in stock.\n"
                        + "*************************************************************************\n" +
                        "Thank you for using our boxes. Good bye next time :)\n" +
                        "*************************************************************************\n";

                    if (UpdateStock(foundDesiredBaseX, foundDesiredHeightY, "single", out bool IsDeleted) && IsDeleted)
                        return StandardMessage + "\n\n!!The used box was the last of its kind and this box type was annulled!!\n";
                    else if (UpdateStock(foundDesiredBaseX, foundDesiredHeightY, "single", out IsDeleted) && !IsDeleted && foundDesiredHeightY.Stock <= ThresholdStock)
                        return StandardMessage + "\n\n!!The used box is prone to be removed from the stock due to few pieces left.!!\n";
                    else if (!UpdateStock(foundDesiredBaseX, foundDesiredHeightY, "single", out IsDeleted))
                        return "\n !!Unexpected error!!\n";
                    else
                        return StandardMessage;
                }
                else
                {
                    return "\n*************************************************************************\n" +
                        "Unfortunately, there was no suitable box found in stock. Try again later :)\n" +
                        "*************************************************************************\n";
                }
            }
            else
            {
                return "\n*************************************************************************\n" +
                    "The stock is empty.\n" +
                    "*************************************************************************\n";
            }
        }

        bool UpdateStock(BoxBase foundDesiredBaseX, BoxHeight foundDesiredHeightY, string mode, out bool IsDeleted)
        {
            IsDeleted = false;
            if (BoxBaseTree.IsIncluded(foundDesiredBaseX, "exact", out foundDesiredBaseX))
            {
                if (foundDesiredBaseX.BoxHeightTree.IsIncluded(foundDesiredHeightY, "exact", out foundDesiredHeightY))
                {
                    if (mode == "single" && foundDesiredHeightY.Stock > 1)
                    {
                        foundDesiredHeightY.Stock -= 1;
                        UpdateTimeStamp(foundDesiredBaseX, foundDesiredHeightY);
                        IsDeleted = false;
                        return true;
                    }
                    else
                    {
                        if (foundDesiredBaseX.BoxHeightTree.CountLevels() > 1)
                        {
                            foundDesiredBaseX.BoxHeightTree.Remove(foundDesiredHeightY);
                        }
                        else
                        {
                            foundDesiredBaseX.BoxHeightTree.Remove(foundDesiredHeightY);
                            BoxBaseTree.Remove(foundDesiredBaseX);
                        }
                        QueueList.RemoveNode(foundDesiredHeightY.timeNodeReference);
                        IsDeleted = true;
                        return true;
                    }
                }
                else
                    return false;
            }
            return false;
        }

        bool UpdateTimeStamp(BoxBase xValue, BoxHeight yValue)
        {
            if (QueueList.MoveNodeToQueueYoung(yValue.timeNodeReference))
            {
                yValue.timeNodeReference.Value.timeStamp = DateTime.Now;
                return true;
            }
            return false;
        }

        public void RemoveFromStockAfterTimeOut(Object o)
        {
            TimeData TimeNode;
            DateTime presentTime = DateTime.Now;
            const int ExpiryTime = 30;

            while (true)
            {
                QueueList.GetFirst(out TimeNode);
                if (presentTime.Subtract(TimeNode.timeStamp).TotalMinutes > ExpiryTime)
                {
                    QueueList.DeQueue(out TimeNode);
                    UpdateStock(TimeNode.xValue, TimeNode.yValue, "all", out bool IsDeleted);
                    Console.WriteLine($"\n!*!* A box-set ({TimeNode.yValue.Stock} pieces) of {TimeNode.xValue.XValue} x {TimeNode.xValue.XValue} x {TimeNode.yValue.YValue} " +
                        $"was removed from the stock due to lack of requests. !*!*");
                }
                else break;
            }
        }
    }
}
