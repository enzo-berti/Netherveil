using DialogueSystem.Runtime;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor
{
    public class GraphSaveUtility
    {
        private DialogueTreeView targetGraphView;

        private DialogueTree tree => targetGraphView.tree;
        private List<Edge> edges => targetGraphView.edges.ToList();
        private List<NodeView> nodes => targetGraphView.nodes.ToList().Cast<NodeView>().ToList();

        public static GraphSaveUtility GetInstance(DialogueTreeView _targetGraphView)
        {
            return new GraphSaveUtility
            {
                targetGraphView = _targetGraphView
            };
        }

        public void SaveGraph()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save dialogue tree", "new Dialogue", "asset",
                "Please enter a file name to save your Dialogue Tree");

            SaveGraph(path);
        }

        public void SaveGraph(string path)
        {
            if (!SaveNodes()) return;

            // Check if the asset already exists
            DialogueTree existingTree = AssetDatabase.LoadAssetAtPath<DialogueTree>(path);

            if (existingTree != null)
            {
                // Asset already exists, update its properties
                EditorUtility.CopySerialized(tree, existingTree);
                AssetDatabase.SaveAssets();
            }
            else
            {
                // Asset doesn't exist, create a new one
                AssetDatabase.CreateAsset(tree, path);
                AssetDatabase.SaveAssets();
            }
        }

        private bool SaveNodes()
        {
            tree.root = null;
            tree.nodes.ToList().ForEach(n => tree.DeleteNode(n));

            // Root
            //RootNodeView rootView = nodes.Where(x => x is RootNodeView).First() as RootNodeView;

            foreach (NodeView nodeView in nodes)
            {
                {
                    RootNodeView rootView = nodeView as RootNodeView;
                    if (rootView != null)
                    {
                        RootNode root = tree.CreateNode(nodeView.type) as RootNode;
                        root.GUID = nodeView.GUID;
                        root.position = nodeView.GetPosition().position;
                        tree.root = root;
                        continue;
                    }
                }

                {
                    DialogueNodeView dialogueView = nodeView as DialogueNodeView;
                    if (dialogueView != null)
                    {
                        DialogueNode dialogue = tree.CreateNode(nodeView.type) as DialogueNode;
                        dialogue.dialogueData = (nodeView as DialogueNodeView).DialogueData;
                        dialogue.GUID = nodeView.GUID;
                        dialogue.position = nodeView.GetPosition().position;
                        continue;
                    }
                }
            }

            edges.ForEach(edge =>
            {
                NodeView outputNodeView = edge.output.node as NodeView;
                NodeView inputNodeView = edge.input.node as NodeView;

                Runtime.Node outputNode = tree.nodes.Find(x => x.GUID == outputNodeView.GUID);
                Runtime.Node inputNode = tree.nodes.Find(x => x.GUID == inputNodeView.GUID);

                if (outputNode is RootNode)
                    (outputNode as RootNode).child = inputNode;
                if (outputNode is SimpleDialogueNode)
                    (outputNode as SimpleDialogueNode).child = inputNode;
                if (outputNode is ChoiceDialogueNode)
                    (outputNode as ChoiceDialogueNode).AddOption(edge.output.portName, inputNode);
            });

            return true;
        }

        public void LoadGraph()
        {
            LoadNodes();
            LinkNodes();
        }

        private void LoadNodes()
        {
            tree.nodes.ForEach(n =>
            {
                targetGraphView.CreateNode(n, n.position);
            });
        }

        private void LinkNodes()
        {
            tree.nodes.ForEach(n =>
            {
                RootNode root = n as RootNode;
                SimpleDialogueNode simple = n as SimpleDialogueNode;
                ChoiceDialogueNode choice = n as ChoiceDialogueNode;

                List<NodeView> nodesGraph = nodes.ToList().Cast<NodeView>().ToList();

                if (root != null && root.child != null)
                {
                    Edge edge = nodesGraph.Find(x => x.GUID == root.GUID).outputContainer[0].Q<Port>()
                        .ConnectTo(nodesGraph.Find(x => x.GUID == root.child.GUID).inputContainer[0].Q<Port>());
                    targetGraphView.AddElement(edge);
                }
                else if (simple != null && simple.child != null)
                {
                    Edge edge = nodesGraph.Find(x => x.GUID == simple.GUID).outputContainer[0].Q<Port>()
                        .ConnectTo(nodesGraph.Find(x => x.GUID == simple.child.GUID).inputContainer[0].Q<Port>());
                    targetGraphView.AddElement(edge);
                }
                else if (choice != null && choice.options.Any())
                {
                    for (int i = 0; i < choice.options.Count; i++)
                    {
                        ChoiceDialogueNodeView outputNodeView = nodesGraph.Find(x => x.GUID == choice.GUID) as ChoiceDialogueNodeView;
                        NodeView inputNodeView = nodesGraph.Find(x => x.GUID == choice.options[i].child.GUID);

                        outputNodeView.AddChoicePort(choice.options[i].option);

                        Edge edge = outputNodeView.outputContainer[i].Q<Port>()
                            .ConnectTo(inputNodeView.inputContainer[0].Q<Port>());
                        targetGraphView.AddElement(edge);
                    }
                }
            });
        }
    }
}
