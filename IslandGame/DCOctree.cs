using System;
using OpenTK;

namespace IslandGame {

    class Octree {

        private Node root;

        public Octree(Vector3 position, float size, int resolution) {
            root = createNode(position, size, resolution);
        }

        private Node createNode(Vector3 pos, float size, int resolution) {
            if (resolution == 1)
                return createLeaf(pos, size);

            InternalNode node = new InternalNode(size, pos);
            float childSize = size / 2;

            for (int i = 0; i < 8; i++) {
                node.SetChild(i, createNode(new Vector3(i / 4 - 0.5f, i % 4 / 2 - 0.5f, i % 2 - 0.5f) * childSize + pos, childSize, resolution - 1));
            }
            
            if (node.IsEmpty()) {
                return new EndNode(size, pos);
            } else {
                return node;
            }

        }

        private Node createLeaf(Vector3 pos, float size) {
            return new LeafNode(size, pos);
        }


        private abstract class Node {

            private float size;
            private Vector3 pos;

            public Node(float size, Vector3 pos) {
                this.size = size;
                this.pos = pos;
            }

            public float Size {
                get {
                    return size;
                }
            }

            public Vector3 Position {
                get {
                    return pos;
                }
            }

            public abstract bool IsEmpty();

        }

        private class EndNode : Node {

            public EndNode(float size, Vector3 pos) : base(size, pos) {

            }

            public override bool IsEmpty() {
                return true;
            }
        }

        private class InternalNode : Node {

            private Node[] children;

            public InternalNode(float size, Vector3 pos) : base(size, pos) {
                children = new Node[8];
            }

            public Node GetChild(int nr) {
                return children[nr];
            }

            public void SetChild(int nr, Node child) {
                children[nr] = child;
            }

            public override bool IsEmpty() {
                foreach (Node oe in children) {
                    if (!oe.IsEmpty()) {
                        return false;
                    }
                }
                return true;
            }
        }

        private class LeafNode : Node {

            public LeafNode(float size, Vector3 pos) : base(size, pos) {
                Console.WriteLine(pos);
            }

            public override bool IsEmpty() {
                return false;
            }
        }

    }

}
