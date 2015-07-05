﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Blocks
{
    /// <summary>
    /// 存储索引的块。此块在内存中充当skip list。
    /// </summary>
    [Serializable]
    public class IndexBlock : Block, IDoubleLinkedNode<IndexBlock>
    {

        /// <summary>
        /// 此Index的第一个Skip List结点（this.SkipListHeadNodes.Last()）的位置。
        /// </summary>
        public long SkipListHeadNodePos { get; set; }

        /// <summary>
        /// 此索引的第一列skip list结点。是skip list的头结点。
        /// 用于skip list的增删。
        /// <para>SkipListHeadNodes[0]是最下方的结点。</para>
        /// <para>/*SkipListNodes[maxLevel - 1]↓*/</para>
        /// <para>/*SkipListNodes[.]↓*/</para>
        /// <para>/*SkipListNodes[.]↓*/</para>
        /// <para>/*SkipListNodes[2]↓*/</para>
        /// <para>/*SkipListNodes[1]↓*/</para>
        /// <para>/*SkipListNodes[0] */</para>
        /// </summary>
        public SkipListNodeBlock[] SkipListHeadNodes { get; set; }

        internal override bool ArrangePos()
        {
            bool allArranged = true;

            if (this.SkipListHeadNodes != null)// 如果这里的SkipListHeadNodes == null，则说明此索引块是索引链表里的头结点。头结点是不需要SkipListHeadNodes有数据的。
            {
                if (this.SkipListHeadNodePos == 0)// 尚未被赋值。
                {
                    int length = this.SkipListHeadNodes.Length;
                    if (length == 0)
                    { throw new Exception("SKip List's head nodes has 0 element!"); }
                    long pos = this.SkipListHeadNodes[length - 1].ThisPos;
                    if (pos != 0)
                    { this.SkipListHeadNodePos = pos; }
                    else
                    { allArranged = false; }
                }
            }

            if (this.NextObj != null)
            {
                if (this.NextPos == 0)// 尚未被赋值。
                {
                    if (this.NextObj.ThisPos != 0)
                    { this.NextPos = this.NextObj.ThisPos; }
                    else
                    { allArranged = false; }
                }
            }

            return allArranged;
        }

        /// <summary>
        /// 用于加速skip list的增删。
        /// </summary>
        internal int CurrentLevel { get; set; }

        /// <summary>
        /// 此Index代表的表的成员（字段/属性）名。
        /// </summary>
        public string BindMember { get; set; }

        /// <summary>
        /// 存储索引的块。
        /// </summary>
        public IndexBlock() { }

        const string strSkipListNodePos = "S";
        const string strCurrentLevel = "C";
        const string strBindMember = "M";

        const string strNext = "N";

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue(strSkipListNodePos, this.SkipListHeadNodePos);
            info.AddValue(strCurrentLevel, this.CurrentLevel);
            info.AddValue(strBindMember, this.BindMember);

            info.AddValue(strNext, this.NextPos);
        }

        protected IndexBlock(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            this.SkipListHeadNodePos = info.GetInt64(strSkipListNodePos);
            this.CurrentLevel = info.GetInt32(strCurrentLevel);
            this.BindMember = info.GetString(strBindMember);

            this.NextPos = info.GetInt64(strNext);
        }


        #region IDoubleLinkedNode 成员

        /// <summary>
        /// 数据库中不保存此值。
        /// </summary>
        public long PreviousPos { get; set; }

        /// <summary>
        /// 数据库中保存此值。
        /// </summary>
        public long NextPos { get; set; }

        /// <summary>
        /// 数据库中不保存此值。
        /// </summary>
        public IndexBlock PreviousObj { get; set; }

        /// <summary>
        /// 数据库中不保存此值。
        /// </summary>
        public IndexBlock NextObj { get; set; }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}, SkipListHeadNodePos: {1}, BindMember: {2}, NextPos: {3}, PreviousPos: {4}",
                base.ToString(),
                this.SkipListHeadNodePos,
                this.BindMember,
                this.NextPos,
                this.PreviousPos);
            //return string.Format("{0}, SkipListHeadNodePos: {1}, BindMember: {2}, NextPos: {3}",
            //    base.ToString(),
            //    this.SkipListHeadNodePos,
            //    this.BindMember,
            //    this.NextPos);
        }
    }
}
