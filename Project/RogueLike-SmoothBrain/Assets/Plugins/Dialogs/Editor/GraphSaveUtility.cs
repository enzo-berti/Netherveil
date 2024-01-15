using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using DialogueSystem.Runtime;
using DialogueSystem.Editor.Nodes;

namespace DialogueSystem.Editor
{
    public class GraphSaveUtility
    {
        private DialogueGraphView targetGraphView;
        private DialogueContainer containerCache;

        private List<Edge> edges => targetGraphView.edges.ToList();
        private List<Nodes.Node> nodes => targetGraphView.nodes.ToList().Cast<Nodes.Node>().ToList();

        public static GraphSaveUtility GetInstance(DialogueGraphView _targetGraphView)
        {
            return new GraphSaveUtility
            {
                targetGraphView = _targetGraphView
            };
        }

        public void SaveGraph()
        {
            var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();

            if (!SaveNodes(dialogueContainer)) return;
            SaveExposedProperties(dialogueContainer);

            string path = EditorUtility.SaveFilePanelInProject("Save dialogue tree", "new Dialogue", "asset",
                "Please enter a file name to save your Dialogue Tree");

            AssetDatabase.CreateAsset(dialogueContainer, path);
            AssetDatabase.SaveAssets();
        }

        private bool SaveNodes(DialogueContainer dialogueContainer)
        {
            if (!edges.Any()) return false;

            var connectedPorts = edges.Where(x => x.input.node != null).ToArray();
            for (int i = 0; i < connectedPorts.Length; i++)
            {
                var outputNode = connectedPorts[i].output.node as Nodes.Node;
                var inputNode = connectedPorts[i].input.node as Nodes.Node;

                dialogueContainer.nodeLinks.Add(new NodeLinkData
                {
                    BaseNodeGuid = outputNode.GUID,
                    PortName = connectedPorts[i].output.portName,
                    TargetNodeGuid = inputNode.GUID
                });
            }

            foreach (var node in nodes)
            {
                {
                    RootNode root = node as RootNode;
                    if (root != null)
                        continue;
                }

                {
                    DialogueNode dialogueNode = node as DialogueNode;
                    if (dialogueNode != null)
                    {
                        dialogueContainer.dialogueNodeData.Add(new DialogueNodeData
                        {
                            Guid = dialogueNode.GUID,
                            nameText = dialogueNode.NameText,
                            dialogueText = dialogueNode.DialogueText,
                            illustrationSprite = dialogueNode.IllustrationSprite,
                            position = dialogueNode.GetPosition().position
                        });
                    }
                }
            }

            return true;
        }

        private void SaveExposedProperties(DialogueContainer dialogueContainer)
        {
            dialogueContainer.exposedProperties.AddRange(targetGraphView.exposedProperties);
        }

        public void LoadGraph()
        {
            string path = EditorUtility.OpenFilePanel("Load Dialogue Tree", Application.dataPath, "asset");

            string assetPath = "Assets" + path.Replace(Application.dataPath, "");
            containerCache = AssetDatabase.LoadAssetAtPath<DialogueContainer>(assetPath);

            if (containerCache == null)
            {
                EditorUtility.DisplayDialog("File not Found", "Target dialogue graph file does not exists!", "OK");
                return;
            }

            ClearGraph();
            CreateNodes();
            ConnectNodes();
            CreateExposedProperties();
        }

        private void CreateExposedProperties()
        {
            targetGraphView.ClearBlackBoardAndExposedProperties();
            foreach (var exposedProperty in containerCache.exposedProperties)
            {
                targetGraphView.AddPropertyToBlackBoard(exposedProperty);
            }
        }

        private void ConnectNodes()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                var connections = containerCache.nodeLinks.Where(x => x.BaseNodeGuid == nodes[i].GUID).ToList();
                for (int j = 0; j < connections.Count; j++)
                {
                    var targetNodeGuid = connections[j].TargetNodeGuid;
                    var targetNode = nodes.First(x => x.GUID == targetNodeGuid);

                    LinkNodes(nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);
                }
            }
        }

        private void LinkNodes(Port output, Port input)
        {
            var tempEdge = new Edge
            {
                output = output,
                input = input,
            };

            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);

            targetGraphView.Add(tempEdge);
        }

        private void CreateNodes()
        {
            foreach (var nodeData in containerCache.dialogueNodeData)
            {
                var tempNode = targetGraphView.CreateNode(typeof(ChoicesNode), nodeData.position) as ChoicesNode;
                tempNode.GUID = nodeData.Guid;
                tempNode.NameText = nodeData.nameText;
                tempNode.DialogueText = nodeData.dialogueText;
                tempNode.IllustrationSprite = nodeData.illustrationSprite;

                if (tempNode == null)
                    continue;

                var nodePorts = containerCache.nodeLinks.Where(x => x.BaseNodeGuid == nodeData.Guid).ToList();
                nodePorts.ForEach(x => tempNode.AddChoicePort(x.PortName));
            }
        }

        private void ClearGraph()
        {
            // Set entry points guid back from the save. Discard existing guid.
            nodes.Find(x => x is Nodes.RootNode).GUID = containerCache.nodeLinks[0].BaseNodeGuid;

            foreach (var node in nodes)
            {
                Nodes.RootNode root = node as Nodes.RootNode;
                if (root != null)
                    continue;

                // Remove edges that connected to this node
                edges.Where(x => x.input.node == node).ToList()
                    .ForEach(edge => targetGraphView.RemoveElement(edge));

                // Then remove the node
                targetGraphView.RemoveElement(node);
            }
        }
    }
}
