using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    public TextMeshProUGUI battleText;
    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI enemyHPText;

    public Button attackButton;
    public Button defendButton;

    public BattleManager manager;

    public Button artsButton;

    public GameObject skillPanel;              // Panel chứa danh sách kỹ năng
    public Transform skillContentPanel;        // Content con có layout group
    public GameObject skillItemButtonPrefab;   // Prefab nút kỹ năng

    void Start()
    {
        ShowMessage("Player Turn");
        skillPanel.SetActive(false);

        artsButton.onClick.RemoveAllListeners();
        artsButton.onClick.AddListener(OnArtsPressed);
    }
    Queue<string> messageQueue = new Queue<string>();
    bool isShowingMessage = false;

    public void ShowMessage(string msg)
    {
        messageQueue.Enqueue(msg);
        if (!isShowingMessage)
            StartCoroutine(ProcessMessageQueue());
    }
    IEnumerator TypeText(string msg)
    {
        battleText.text = "";
        foreach (char c in msg)
        {
            battleText.text += c;
            yield return new WaitForSeconds(0.02f);
        }
    }
    IEnumerator ProcessMessageQueue()
    {
        isShowingMessage = true;

        while (messageQueue.Count > 0)
        {
            string msg = messageQueue.Dequeue();
            yield return StartCoroutine(TypeText(msg));
            yield return new WaitForSeconds(0.8f);
        }

        isShowingMessage = false;
    }

    public void UpdateHP(int playerHP, int enemyHP)
    {
        playerHPText.text = $"Player HP: {playerHP}";
        enemyHPText.text = $"Enemy HP: {enemyHP}";
    }

    public void UpdateTeamHP(List<CharacterStats> playerTeam, List<CharacterStats> enemyTeam)
    {
        int totalPlayerHP = playerTeam.Sum(c => c.currentHP);
        int totalEnemyHP = enemyTeam.Sum(c => c.currentHP);

        playerHPText.text = $"Player Team HP: {totalPlayerHP}";
        enemyHPText.text = $"Enemy Team HP: {totalEnemyHP}";
    }

    public void SetupPlayerTurn(BattleUnit unit)
    {
        ShowMessage("Lượt của: " + unit.stats.name);

        attackButton.onClick.RemoveAllListeners();
        defendButton.onClick.RemoveAllListeners();

        attackButton.onClick.AddListener(() => manager.PlayerAttack());
        defendButton.onClick.AddListener(() => manager.PlayerDefend());
    }

    private void OnArtsPressed()
    {
        BattleItemManager.Instance.Close();
        var cu = manager.CurrentUnit;
        if (cu == null || !cu.isPlayer)
        {
            ShowMessage("Không phải lượt player.");
            return;
        }

        var skills = manager.GetSkillsForCharacter(cu.stats.id);

        if (skills == null || skills.Count == 0)
        {
            ShowMessage("Không có Arts khả dụng.");
            return;
        }

        ShowArtsList(skills);
    }

    public SkillIconDatabase skillIconDB; 

    public void ShowArtsList(List<SkillData> skills)
    {
        foreach (var s in skills)
        {
            Debug.Log($"[DEBUG SKILL] name={s.name} | id={s.id}");
        }

        if (skills == null)
        {
            Debug.LogError("[ShowArtsList] Danh sách kỹ năng bị null!");
            return;
        }

        Debug.Log($"[ShowArtsList] Bắt đầu hiển thị {skills.Count} kỹ năng");

        if (skillContentPanel == null)
        {
            Debug.LogError("[ShowArtsList] skillContentPanel bị null!");
            return;
        }

        if (skillPanel == null)
        {
            Debug.LogError("[ShowArtsList] skillPanel bị null!");
            return;
        }

        if (skillItemButtonPrefab == null)
        {
            Debug.LogError("[ShowArtsList] skillItemButtonPrefab bị null!");
            return;
        }

        Debug.Log($"[ShowArtsList] Prefab: {skillItemButtonPrefab.name}, Parent: {skillContentPanel.name}");

        // Dọn các nút cũ trong Content panel
        int oldCount = skillContentPanel.childCount;
        Debug.Log($"[ShowArtsList] Xóa {oldCount} nút cũ trong content panel");

        for (int i = oldCount - 1; i >= 0; i--)
        {
            var child = skillContentPanel.GetChild(i);
            Debug.Log($"[ShowArtsList] Xóa nút: {child.name}");
            Destroy(child.gameObject);
        }

        // Tạo nút cho từng kỹ năng
        int createdCount = 0;
        foreach (var s in skills)
        {
            var item = Instantiate(skillItemButtonPrefab, skillContentPanel);
            if (item == null)
            {
                Debug.LogError($"[ShowArtsList] Instantiate thất bại cho kỹ năng: {s.name}");
                continue;
            }
            var skillBtn = item.GetComponent<SkillButton>();
            if (skillBtn != null)
            {
                skillBtn.skill = s;
                Debug.Log($"[TOOLTIP] Đã gán skill {s.name} cho button");
            }
            else
            {
                Debug.LogError("Prefab SkillItemButton thiếu SkillButton component!");
            }

            Debug.Log($"[ShowArtsList] Đã tạo nút: {item.name}");

            // Lấy icon từ DB theo skillId
            var iconImage = item.GetComponentInChildren<Image>();
            if (iconImage != null)
            {
                var icon = skillIconDB != null ? skillIconDB.GetIconById(s.id) : null;
                if (icon != null)
                {
                    iconImage.sprite = icon;
                    iconImage.enabled = true;
                    Debug.Log($"[ShowArtsList] Gán icon cho skill {s.name} (id={s.id})");
                }
                else
                {
                    Debug.LogWarning($"[ShowArtsList] Không tìm thấy icon cho skill id={s.id}");
                }
            }
            var btn = item.GetComponent<Button>();
            if (btn != null)
            {
                btn.enabled = true;
                btn.interactable = true;
                btn.onClick.AddListener(() =>
                {
                    Debug.Log("CLICK SKILL BUTTON: " + s.name);
                    if (TooltipManager.Instance != null)
                        TooltipManager.Instance.HideTooltip();
                    manager.UseSkill(s);
                    HideArtsList();
                });

            }
            else
            {
                Debug.LogWarning($"[ShowArtsList] Không tìm thấy Button trong prefab cho kỹ năng: {s.name}");
            }

            createdCount++;
        }

        Debug.Log($"[ShowArtsList] Đã tạo tổng cộng {createdCount} nút kỹ năng");

        skillPanel.SetActive(true);
        Debug.Log("[ShowArtsList] skillPanel đã được bật");

    }

    public void HideArtsList()
    {
        skillPanel.SetActive(false);
        if (TooltipManager.Instance != null)
            TooltipManager.Instance.HideTooltip();
    }
    public void ResetPanels()
    {
        HideArtsList();

        if (BattleItemManager.Instance != null)
        {
            BattleItemManager.Instance.Close();
        }
    }
}