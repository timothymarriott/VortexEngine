using System.Numerics;
using System.Reflection;
using ImGuiNET;
using VortexEngine.Internal.AssetManagement;

namespace VortexEngine.Rendering;

public static class EditorGui
{

    public static bool SelectIncludeAssets;

    public static void DrawObjectProperty(object Obj, string propertyName){
        Type self = Obj.GetType();


        FieldInfo property = self.GetField(propertyName);
        
            Type type = property.FieldType;

            if(property.GetCustomAttribute<HideInInspector>() != null){
                return;
            }


            if (type == typeof(string)){
                string value = (string)property.GetValue(Obj);
                ImGui.InputText(property.Name, ref value, 100);

                property.SetValue(Obj, value);
            }
            if (type == typeof(int)){
                int value = (int)property.GetValue(Obj);
                ImGui.DragInt(property.Name, ref value);
                property.SetValue(Obj, value);
            }
            if (type == typeof(float)){
                float value = (float)property.GetValue(Obj);
                ImGui.DragFloat(property.Name, ref value);
                property.SetValue(Obj, value);
            }
            if (type == typeof(Vector2))
            {
                Vector2 v = (Vector2)property.GetValue(Obj);
                System.Numerics.Vector2 value = v;
                ImGui.DragFloat2(property.Name, ref value);
                if (v != value)
                    property.SetValue(Obj, new Vector2(value.X, value.Y));
            }
            if (type == typeof(Vector2I)){
                System.Numerics.Vector2 value = (Vector2I)property.GetValue(Obj);
                int hah = (int)value.X;
                ImGui.InputFloat2(property.Name, ref value);
                property.SetValue(Obj, new Vector2I((int)value.X, (int)value.Y));
            }
            if (type == typeof(bool)){
                bool value = (bool)property.GetValue(Obj);
                ImGui.Checkbox(property.Name, ref value);
                property.SetValue(Obj, value);
            }
            if (type == typeof(Color)){
                Color _color = (Color)property.GetValue(Obj);
                System.Numerics.Vector4 color = _color;
                ImGui.ColorEdit4(property.Name, ref color);
                property.SetValue(Obj, (Color)color);
            }
            if (type == typeof(Body)){
                ImGui.Text(property.Name);
                ImGui.SameLine();
                Body value = (Body)property.GetValue(Obj);
                if (value != null){
                    ImGui.PushID(value.ID);
                    if (ImGui.Button(value.Name)){
                        if (!ImGui.IsPopupOpen("popup_body_select_" + property.Name)){
                            ImGui.OpenPopup("popup_body_select_" + property.Name);
                        }

                    }
                    ImGui.PopID();
                } else {
                    if (ImGui.Button("None")){
                    if (!ImGui.IsPopupOpen("popup_body_select_" + property.Name)){
                        ImGui.OpenPopup("popup_body_select_" + property.Name);
                    }

                }
                }

                if (ImGui.BeginPopup("popup_body_select_" + property.Name)){

                    ImGui.Checkbox("Include Assets", ref SelectIncludeAssets);
                    foreach (var item in VortexEngine.Master.LoadedScene.Bodys)
                    {
                        ImGui.PushID(item.ID);
                        if (ImGui.Button(item.Name)){
                            ImGui.CloseCurrentPopup();
                            property.SetValue(Obj, item);
                        }
                        ImGui.PopID();

                    }
                    ImGui.PushID("NONE_BODY_SELECT");
                    if (ImGui.Button("None")){
                        ImGui.CloseCurrentPopup();
                        property.SetValue(Obj, null);
                    }
                    ImGui.PopID();

                    ImGui.EndPopup();
                }
            }
            if (VortexEngine.GetSubclassesOf(typeof(Component)).Contains(type)){
                ImGui.Text(property.Name);
                ImGui.SameLine();
                Component value = (Component)property.GetValue(Obj);
                if (value != null){
                    if (value.body != null)
                    {
                        ImGui.PushID(value.body.ID);
                        if (ImGui.Button(value.body.Name)){
                            if (!ImGui.IsPopupOpen("popup_body_select_" + property.Name)){
                                ImGui.OpenPopup("popup_body_select_" + property.Name);
                            }

                        }
                        ImGui.PopID();
                    }
                    else
                    {
                        ImGui.TextColored(new Vector4(1, 0, 0, 1), "Corrupted");
                    }
                    
                } else {
                    if (ImGui.Button("None")){
                    if (!ImGui.IsPopupOpen("popup_body_select_" + property.Name)){
                        ImGui.OpenPopup("popup_body_select_" + property.Name);
                    }

                }
                }

                if (ImGui.BeginPopup("popup_body_select_" + property.Name)){

                    ImGui.Checkbox("Include Assets", ref SelectIncludeAssets);
                    foreach (var item in VortexEngine.Master.LoadedScene.Bodys.Where(x => x.GetComponent(type) != null))
                    {
                        ImGui.PushID(item.ID);
                        if (ImGui.Button(item.Name)){
                            ImGui.CloseCurrentPopup();
                            property.SetValue(Obj, item.GetComponent(type));
                        }
                        ImGui.PopID();

                    }

                    ImGui.EndPopup();
                }
            }
            if (type == typeof(Prefab)){
                ImGui.Text(property.Name);
                ImGui.SameLine();
                Prefab value = (Prefab)property.GetValue(Obj);
                if (value != null){
                    if (ImGui.Button(value.FileHandle)){
                        if (!ImGui.IsPopupOpen("popup_scene_select_" + property.Name)){
                            ImGui.OpenPopup("popup_scene_select_" + property.Name);
                        }

                    }
                } else {
                    if (ImGui.Button("None")){
                        if (!ImGui.IsPopupOpen("popup_scene_select_" + property.Name)){
                            ImGui.OpenPopup("popup_scene_select_" + property.Name);
                        }

                    }
                }

                if (ImGui.BeginPopup("popup_scene_select_" + property.Name)){

                    foreach (var item in AssetManager.AvailablePrefabIds)
                    {
                        if (ImGui.Button(item)){
                            ImGui.CloseCurrentPopup();
                            property.SetValue(Obj, Prefab.GetFromHandle(item));

                        }

                    }

                    ImGui.EndPopup();
                }
            }
        try {
        } catch (Exception e){
            Console.WriteLine("Error drawing: " + property.Name + " : " + e.Message);
        }


    }

    public static void BeginBorderedArea(){
        ImDrawListPtr drawlist = ImGui.GetWindowDrawList();


        ImGui.BeginGroup();
    }

    public static void EndBorderedArea(Color color){
        



        ImGui.EndGroup();


        Vector2 rectMax = ImGui.GetItemRectMax();
        rectMax.x -= ImGui.GetItemRectSize().X;
        rectMax.x += ImGui.GetContentRegionAvail().X + 4;

        Vector2 rectMin = ImGui.GetItemRectMin();
        rectMin.x -= 4;
        
        ImGui.GetWindowDrawList().AddRect(rectMin, rectMax, ImGui.GetColorU32(ImGuiCol.Button), 4, ImDrawFlags.RoundCornersAll, 3);
        

    }

    public static void DrawObject(object Obj){
        Type self = Obj.GetType();


        Performance.PushTask("Draw Props");
        foreach(FieldInfo property in self.GetFields()){
            Performance.PushTask($"({property.FieldType.Name}) {property.Name}");
            DrawObjectProperty(Obj, property.Name);
            Performance.PopTask();

        }
        Performance.PopTask();
        
        Performance.PushTask("Validate");
        if (Obj is Component comp){
            comp.OnValidate();
        }
        Performance.PopTask();
        

    }
}
