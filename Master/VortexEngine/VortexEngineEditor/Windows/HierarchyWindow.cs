using ImGuiNET;
using VortexEngine.Rendering;

namespace VortexEngine.Editor.Windows;

public class HierarchyWindow : EditorWindow
{
    
    public Dictionary<string, bool> ExpandedInInspector = new Dictionary<string, bool>();

    public override string GetTitle()
    {
        return "Hierarchy";
    }


    public override void DrawContent(VortexEngineEditor editor)
    {
        /*
        if (ImGui.BeginMenuBar()){
            if (ImGui.BeginMenu("Add")){
                if (ImGui.MenuItem("New Object")){
                    editor.LoadedScene.Bodys.Add(new Body());
                }
                ImGui.EndMenu();

            }
            ImGui.EndMenuBar();
        }
        */


        if (editor.LoadedScene != null){
            foreach (Body body in editor.LoadedScene.Bodys)
            {
                if (body.transform.Parent == null)
                    DrawHierarchyBody(body);
            }
        }
    }

    public override ImGuiWindowFlags GetFlags()
    {
        return ImGuiWindowFlags.MenuBar;
    }
    
    private void DrawHierarchyBody(Body body)
    {

        if (!ExpandedInInspector.ContainsKey(body.ID)){
            ExpandedInInspector.Add(body.ID, false);
        }
        
        ImGui.PushID(body.ID);
        if (ImGui.Button(body.Name)){
            VortexEngineEditor.Editor.SelectedBody = body;
        }

        /*
        if (body.transform.Children.Count > 0){
            ImGui.SameLine();
            if (ImGui.ArrowButton(body.ID, ExpandedInInspector[body.ID] ? ImGuiDir.Down : ImGuiDir.Right)){
                ExpandedInInspector[body.ID] = !ExpandedInInspector[body.ID];
            }
        }

        if (ExpandedInInspector[body.ID]){
            ImGui.Indent();

            foreach (var child in body.transform.Children)
            {
                DrawHierarchyBody(child);
            }
            ImGui.Unindent();
        }
        */
        
        ImGui.PopID();
    }
    
}