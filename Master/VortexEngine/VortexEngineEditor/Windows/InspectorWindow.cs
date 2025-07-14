using ImGuiNET;

namespace VortexEngine.Editor.Windows;

public class InspectorWindow : EditorWindow
{
    public override string GetTitle()
    {
        return "Inspector";
    }

    public override void DrawContent(VortexEngineEditor editor)
    {
        if (editor.SelectedBody != null){
            if (editor.LoadedScene != null){
                if (!editor.LoadedScene.Bodys.Contains(editor.SelectedBody)){
                    editor.SelectedBody = null;

                } else {
                    DrawBodyInspector(editor.SelectedBody);
                }
            }
        }

        if (editor.SelectedBody != null){
            if (ImGui.Button("Add Component")){
                if (!ImGui.IsPopupOpen("body_addcomponent")){
                    ImGui.OpenPopup("body_addcomponent");
                }

            }

            ImGui.PushStyleColor(ImGuiCol.Button, Color.Red);
            if (ImGui.Button("Destroy")){
                ImGui.OpenPopup("body_destroy");
            }

            ImGui.PopStyleColor();

            if (ImGui.BeginPopupModal("body_destroy")){
                ImGui.Text("Are you sure you want to delete " + editor.SelectedBody.Name);
                ImGui.Spacing();
                if (ImGui.Button("Cancel")){
                    ImGui.CloseCurrentPopup();
                }
                ImGui.SameLine();

                if (ImGui.Button("Destroy")){
                    editor.SelectedBody.OnDestroyed();
                    editor.LoadedScene.Bodys.Remove(editor.SelectedBody);
                    editor.SelectedBody = null;
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndPopup();

            }

            if (ImGui.BeginPopup("body_addcomponent")){
                foreach (var component in VortexEngine.GetSubclassesOf(typeof(Component)))
                {
                    if (ImGui.Button(component.Name)){
                        Component comp = VortexEngineEditor.Editor.SelectedBody.AddComponent(component);
                        comp.Awake();
                    }
                }
                ImGui.EndPopup();
            }
        }
    }
    
    public void DrawBodyInspector(Body body){
        ImDrawListPtr drawlist = ImGui.GetWindowDrawList();

        #region BodyHeader

        ImGui.BeginGroup();

        bool collapsed = ImGui.CollapsingHeader("Body Options", ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.Framed);

        if (ImGui.BeginPopupContextItem("bodyoptions_" + body.Name + " - " + body.ID)){
            ImGui.EndPopup();
        }

        if (collapsed){
            ImGui.InputText("Name", ref body.Name, 128);

            ImGui.Spacing();
        }
        
        ImGui.EndGroup();
        
        Vector2 rectMax = ImGui.GetItemRectMax();
        rectMax.x -= ImGui.GetItemRectSize().X;
        rectMax.x += ImGui.GetContentRegionAvail().X + 4;

        Vector2 rectMin = ImGui.GetItemRectMin();
        rectMin.x -= 4;
        if (collapsed){
            ImGui.GetWindowDrawList().AddRect(rectMin, rectMax, ImGui.GetColorU32(ImGuiCol.Border), 4, ImDrawFlags.RoundCornersAll, 3);
        }
        #endregion
        
        int index = 0;

        Component toRemove = null;

        foreach (Component comp in body.Components)
        {
            Performance.PushTask(comp.GetType().Name);

            ImGui.PushID($"BODY_{body.ID}_COMP_{comp.GetType().Name}_{index}");
            ImGui.BeginGroup();

            collapsed = ImGui.CollapsingHeader(comp.GetType().Name, ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.Framed);

            if (ImGui.BeginPopupContextItem("component_" + comp.GetType().Name + " - " + comp.body.ID)){
                if (ImGui.MenuItem("Remove")){
                    toRemove = comp;
                }
                ImGui.EndPopup();
            }

            if (collapsed){
                comp.DrawGui();
                ImGui.Spacing();
            }
            
            ImGui.EndGroup();
            
            rectMax = ImGui.GetItemRectMax();
            rectMax.x -= ImGui.GetItemRectSize().X;
            rectMax.x += ImGui.GetContentRegionAvail().X + 4;

            rectMin = ImGui.GetItemRectMin();
            rectMin.x -= 4;
            if (collapsed){
                ImGui.GetWindowDrawList().AddRect(rectMin, rectMax, ImGui.GetColorU32(ImGuiCol.Border), 4, ImDrawFlags.RoundCornersAll, 3);
            }
            
            ImGui.PopID();

            index++;

            Performance.PopTask();
        }
        
        if (toRemove != null){
            body.Components.Remove(toRemove);
        }
    }
}