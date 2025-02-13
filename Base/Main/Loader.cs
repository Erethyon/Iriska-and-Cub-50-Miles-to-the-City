using Godot;
using System;
using System.Linq;

public partial class Loader : Node
{
    [ExportCategory("Map")]
    [Export] private PackedScene mapScene;
    private BaseMap mapNode;
    [ExportCategory("Player stuff")]
    [Export] private PackedScene playerScene;
    private Player playerNode;
    private Node2D playerSpawnPoint;
    private PlayerCamera playerCamera;
    [Export] private PackedScene gameUIScene;
    private Control gameUI;
    [Export] private PackedScene? clientScene;

    public PackedScene MapScene => mapScene;
    public Node2D MapNode => mapNode;
    public PackedScene PlayerScene => playerScene;
    public Player PlayerNode => playerNode;
    public PlayerCamera PlayerCamera => playerCamera;
    public PackedScene ClientScene => clientScene;
    public PackedScene GameUIScene => gameUIScene;
    public Control GameUI => gameUI;


    public override void _Ready()
    {
        GetTree().Root.GetChildren();
        mapNode = mapScene.Instantiate<BaseMap>();
        AddChild(MapNode);

        playerNode = playerScene.Instantiate<Player>();
        playerSpawnPoint = mapNode.GetNode<Node2D>("PlayerSpawnPoint");
        playerNode.GlobalPosition = playerSpawnPoint.GlobalPosition;
        AddChild(PlayerNode, true);

        gameUI = gameUIScene.Instantiate<Control>();
        try{
            playerCamera = playerNode.GetChildren().OfType<PlayerCamera>().First();
            playerCamera.AddChild(GameUI);
        }
        catch(Exception){ GD.PrintErr("При попытке заспавнить игрока, у него не была найдена камера");};
        
        //
        foreach (Node node in GetTree().GetNodesInGroup("Enemy")){
            ((NPC)node).TargetNode = playerNode;
            GD.Print($"Property DirectTargetNode has been set to {playerNode.Name} for {node.Name}");
        }
        StaticExtensions.CheckPublicMembersForNull_Node<Node, Loader>(this);
    }
}