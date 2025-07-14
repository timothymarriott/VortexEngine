using ImGuiNET;
using rlImGui_cs;
using VortexEngine.Internal.AssetManagement;

namespace VortexEngine.Editor.Windows;
public class FileBrowserWindow : EditorWindow
{
    public override string GetTitle()
    {
        return "Files";
    }

    public override void DrawContent(VortexEngineEditor editor)
    {
        AssetManager.AvailableImageIds.Clear();
        AssetManager.AvailablePrefabIds.Clear();

        DirectoryInfo RootDirInfo = new DirectoryInfo(VortexEngine.ProjectDataPath);

        DirectoryInfo[] ChildDirectories = RootDirInfo.GetDirectories();

        for (int i = 0; i < ChildDirectories.Length; i++)
        {
            CollectDirectoryMeta(ChildDirectories[i]);
        }

        FileInfo[] ChildFiles = RootDirInfo.GetFiles();

        for (int i = 0; i < ChildFiles.Length; i++)
        {
            CollectFileMeta(ChildFiles[i]);
        }

        if (CurrentRenameTarget != null){
            ImGui.OpenPopup("Rename " + CurrentRenameTarget.Name);

            if (ImGui.BeginPopupModal("Rename " + CurrentRenameTarget.Name, ImGuiWindowFlags.AlwaysAutoResize)){

                ImGui.InputText("New Name", ref CurrentRenameValue, 100);
                if (ImGui.Button("Cancel")){
                    CurrentRenameTarget = null;
                    ImGui.CloseCurrentPopup();
                }
                ImGui.SameLine();
                if (ImGui.Button("Rename")){
                    File.Move(CurrentRenameTarget.FullName, CurrentRenameTarget.Directory.FullName + "/" + CurrentRenameValue + CurrentRenameTarget.Extension);
                    CurrentRenameTarget = null;
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndPopup();
            }
        }

    }
    public void CollectDirectoryMeta(DirectoryInfo directory){

        DirectoryInfo[] ChildDirectories = directory.GetDirectories();

        for (int i = 0; i < ChildDirectories.Length; i++)
        {
            CollectDirectoryMeta(ChildDirectories[i]);
        }

        FileInfo[] ChildFiles = directory.GetFiles();

        for (int i = 0; i < ChildFiles.Length; i++)
        {
            CollectFileMeta(ChildFiles[i]);
        }

    }

    public void CollectFileMeta(FileInfo file){
        if (file.Extension == ".png" || file.Extension == ".jpg" || file.Extension == ".jpeg")
        {
            Console.WriteLine("Full name: " + file.FullName);
            string textureId = file.FullName.Replace(VortexEngine.ProjectDataPath, "").Replace(".png", "").Replace(".jpg", "").Replace(".jpeg", "").Replace("\\", "/");
            Console.WriteLine("Texid: " + textureId);
            if (!AssetManager.AvailableImageIds.Contains(textureId)){
                AssetManager.AvailableImageIds.Add(textureId);
            }
        }

        if (file.Extension == ".vobj"){
            string textureId = file.FullName.Replace(VortexEngine.ProjectDataPath, "").Replace(".vobj", "").Replace("\\", "/");
            if (!AssetManager.AvailablePrefabIds.Contains(textureId)){
                AssetManager.AvailablePrefabIds.Add(textureId);
            }
        }
    }

    public void DrawDirectory(DirectoryInfo directory)
    {
        rlImGui.ImageSize(Icons.Get("Folder"), new Vector2(ImGui.GetFontSize() * 2, ImGui.GetFontSize() * 2));

        ImGui.SameLine();

        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + ImGui.GetFontSize() / 2);

        if (ImGui.TreeNode(directory.Name))
        {
            DirectoryContextMenu(directory.FullName, directory);

            DirectoryInfo[] ChildDirectories = directory.GetDirectories();

            for (int i = 0; i < ChildDirectories.Length; i++)
            {
                DrawDirectory(ChildDirectories[i]);
            }

            FileInfo[] ChildFiles = directory.GetFiles();

            for (int i = 0; i < ChildFiles.Length; i++)
            {
                DrawFile(ChildFiles[i]);
            }
            ImGui.TreePop();
        } else {

            DirectoryContextMenu(directory.FullName, directory);
        }

    }


    public FileInfo? CurrentRenameTarget;

    public string CurrentRenameValue = "";


    public void FileContextMenu(string id, FileInfo file){

        if (ImGui.BeginPopupContextItem(id)){
            AddMenu(file.Directory);

            if (ImGui.MenuItem("Rename")){
                
                if (CurrentRenameTarget == null){
                    CurrentRenameTarget = file;
                    CurrentRenameValue = file.Name.Split(file.Extension)[0];

                }

            }
            if (file.Extension == ".vobj"){
                if (ImGui.MenuItem("Create")){
                    string fileid = file.FullName.Replace(VortexEngine.ProjectDataPath, "");
                    VortexEngine.Master.AddSceneAsPrefab(fileid, Vector2.Zero).ID = Guid.NewGuid().ToString();
                }
            }
            
            ImGui.EndPopup();
        }

    }

    public void AddMenu(DirectoryInfo directory){
        if (ImGui.BeginMenu("Add")){
            if (ImGui.MenuItem("Prefab")){
                File.AppendAllText(Path.Join(directory.FullName, "new_prefab.vobj"), "{\"Bodies\":[{\"transform\":{\"Position\":{\"x\":0,\"y\":0},\"Scale\":{\"x\":1,\"y\":1},\"Rotation\":0},\"ID\":\"" + Guid.NewGuid().ToString() + "\",\"Name\":\"NewPrefab\",\"Components\":[{\"type\":{\"Name\":\"Transform\",\"Fullname\":\"VortexEngine.Transform\",\"Assembly\":\"VortexEngineInternal\"},\"Transform\":{\"Position\":{\"x\":0,\"y\":0},\"Scale\":{\"x\":1,\"y\":1},\"Rotation\":0}}],\"Parent\":null,\"Children\":[]}]}");
            }
            ImGui.EndMenu();
        }
    }

    public void DirectoryContextMenu(string id, DirectoryInfo directory){
        if (ImGui.BeginPopupContextItem(id)){
            AddMenu(directory);
            ImGui.EndPopup();
        }
    }

    public void DrawFile(FileInfo file)
    {
        if (file.FullName.EndsWith(".DS_Store")) return;

        string icon = "File";

        if (file.Extension == ".png" || file.Extension == ".jpg" || file.Extension == ".jpeg")
        {
            icon = "Image";

        }

        rlImGui.ImageSize(Icons.Get(icon), new Vector2(ImGui.GetFontSize() * 2, ImGui.GetFontSize() * 2));

        FileContextMenu(file.FullName + "_img", file);

        ImGui.SameLine();

        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + ImGui.GetFontSize() / 2);

        ImGui.Text(file.Name);
        FileContextMenu(file.FullName, file);

        if (file.Extension == ".vobj"){
            if (ImGui.IsItemClicked()){
                string sceneId = file.FullName.Replace(VortexEngine.ProjectDataPath, "").Replace(".vobj", "");
                Console.WriteLine("Opening " + sceneId);
                VortexEngineEditor.Master.LoadScene(sceneId + ".vobj");
            }
        }
        if (file.Extension == ".vscn"){
            if (ImGui.IsItemClicked()){
                string sceneId = file.FullName.Replace(VortexEngine.ProjectDataPath, "").Replace(".vscn", "");
                Console.WriteLine("Opening " + sceneId);
                VortexEngineEditor.Master.LoadScene(sceneId + ".vscn");
            }
        }

    }

}
