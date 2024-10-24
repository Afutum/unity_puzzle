using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    // �A�C�e���̃v���n�u
    [SerializeField] List<GameObject> prefabBubbles;
    // �Q�[������
    [SerializeField] public float gameTimer;
    // �t�B�[���h�̃A�C�e������
    [SerializeField] int fieldItemCountMax;
    // �폜�ł���A�C�e����
    [SerializeField] int deleteCount;
    // �R���{�J�E���g
    [SerializeField] int comboCount;

    EnemyManager enemy;
    UIManager uiManager;
    PlayerManager player;

    // �t�B�[���h��̃A�C�e��
    List<GameObject> bubbles;
    // �X�R�A
    int gameScore;
    // �Đ����u
    AudioSource audioSource;

    // �c���c����
    List<GameObject> lineBubbles;
    LineRenderer lineRenderer;

    // �v���C���[�E�G�̃^�[�����ǂ���
    public bool isPlayerTurn;
    public bool IsPlayerTurn { get { return isPlayerTurn; } }

    bool isEnemyTurn;
    public bool IsEnemyTurn { get { return isEnemyTurn; } }
    // attack�{�^�������������ǂ���
    bool isAttackBtn;
    // �I���������ǂ���
    public bool isEnd;
    public bool IsEnd { get { return isEnd; } }
    // �o�߃��E���h
    int roundCnt = 0;
    public int RoundCnt { get { return roundCnt; } }
    // �U����
    int powerNum = 0;
    public int PowerNum { get { return powerNum; } }
    // �U�������ǂ���
    bool isDelete = false;
    public bool IsDelete { get { return isDelete; } }

    bool isClear;
    public bool IsClear { get { return isClear; } }

    bool isGameOver;
    public bool IsGameOver { get { return isGameOver; } }

    public float GameTimer { get { return gameTimer; } }

    int hideRoundCnt = 0;

    GameObject bubble;

    [SerializeField] AudioClip chainSE;

    // Start is called before the first frame update
    void Start()
    {
        // Enemy�X�N���v�g�̎擾
        enemy = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();

        // UIManager�X�N���v�g�̎擾
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        player = GameObject.Find("player").GetComponent<PlayerManager>();

        audioSource = GetComponent<AudioSource>();

        // Audio
        //audioSource = GetComponent<AudioSource>();

        // �S�A�C�e��
        bubbles = new List<GameObject>();

        // �c���c����
        lineBubbles = new List<GameObject>();
        // �A�������A�C�e����̃��C��
        lineRenderer = GetComponent<LineRenderer>();

        // ���U���g��ʔ�\��
        //gameResult.SetActive(false);

        isPlayerTurn = false;
        isEnemyTurn = true;

        isEnd = false;

        isClear = false;
        isGameOver = false;

        // �A�C�e������
        SpawnItem(fieldItemCountMax);

        ChangTurn();
    }

    // Update is called once per frame
    public void Update()
    {
        // isEnd��true�̂Ƃ�
        if (isEnd)
        {
            Debug.Log("���肠");

            // ���̎��_��Update���甲����
            return;
        }
        else if (isPlayerTurn == true && 
            enemy.EnemyCurrentStatus == EnemyManager.EnemyStatus.None)
        {// isEnd��false�E�G�̃^�[������Ȃ��Ƃ��E�G�̏�Ԃ�None�̂Ƃ�

            if (hideRoundCnt == 0)
            {
                uiManager.RoundUpText(roundCnt);
                hideRoundCnt++;
            }

            // ���쎞�ԍX�V����
            gameTimer -= Time.deltaTime;

            // ���쎞�ԏI���������͍U���{�^���������ꂽ�ꍇ
            if (gameTimer < 0 || isAttackBtn == true)
            {
                if (player.Power > 0)
                {
                    if (player.CompleteAction == null)
                    {
                        player.Attack();

                        player.PushButtonAnim(() =>
                        {
                            // �U�����I��点��
                            isDelete = false;

                            // ���E���h�����G��菭�Ȃ�������
                            if (roundCnt < enemy.Enemy.Length)
                            {
                                // �^�C�}�[��߂�
                                gameTimer = 30;

                                // �U���͂̐��l�����ɖ߂�
                                player.ResetPower();

                                if (enemy.Hp[roundCnt] > 0)
                                {
                                    ChangTurn();
                                }
                                else
                                {
                                    isAttackBtn = true;
                                }
                            }
                            else
                            {
                                // ���U���g��ʕ\��
                                //gameResult.SetActive(true);

                                NetworkManager.Instance.RegistClearStage(StageSelectManager.stageId,
                                result => {
                                    GameEnd();
                                });

                                // Update�ɓ���Ȃ��悤�ɂ���
                                enabled = false;

                                // ���̎��_��Update���甲����
                                return;
                            }
                        });
                    }
                }
                else
                {
                    // �U�����I��点��
                    isDelete = false;

                    // ���E���h����2�ȉ���������
                    if (roundCnt <= enemy.Enemy.Length)
                    {
                        if (enemy.Hp[0] > 0)
                        {
                            ChangTurn();
                        }

                        // �^�C�}�[��߂�
                        gameTimer = 30;

                        // �U���͂̐��l�����ɖ߂�
                        player.ResetPower();
                    }
                    else
                    {
                        // ���U���g��ʕ\��
                        //gameResult.SetActive(true);

                        isClear = true;

                        NetworkManager.Instance.RegistClearStage(StageSelectManager.stageId,
                            result => {
                                GameEnd();
                            });

                        // Update�ɓ���Ȃ��悤�ɂ���
                        enabled = false;

                        // ���̎��_��Update���甲����
                        return;
                    }
                }
            }

            // �^�b�`�J�n
            if (Input.GetMouseButtonDown(0))
            {
                GameObject hitBubble = GetHitBubble();

                // ������
                lineBubbles.Clear();

                // �����蔻��
                if (hitBubble)
                {
                    lineBubbles.Add(hitBubble);
                }
            }
            // �������ςȂ�
            else if (Input.GetMouseButton(0))
            {
                GameObject hitBubble = GetHitBubble();

                // �����蔻�肠��
                if (hitBubble && lineBubbles.Count > 0)
                {
                    // ����
                    GameObject pre = lineBubbles[lineBubbles.Count - 1];
                    float distans =
                        Vector2.Distance(hitBubble.transform.position, pre.transform.position);

                    // �J���[
                    bool isSameColor =
                        hitBubble.GetComponent<SpriteRenderer>().sprite == pre.GetComponent<SpriteRenderer>().sprite;

                    if (isSameColor && distans <= 1.5f && !lineBubbles.Contains(hitBubble))
                    {
                        audioSource.PlayOneShot(chainSE);

                        // ���C���ǉ�
                        lineBubbles.Add(hitBubble);
                    }
                }
            }
            // �^�b�`�I��
            else if (Input.GetMouseButtonUp(0))
            {
                // �폜���ꂽ�A�C�e�����N���A
                bubbles.RemoveAll(item => item == null);

                // �A�C�e���폜
                DeleteItems(lineBubbles);

                // ���C�����N���A
                lineRenderer.positionCount = 0;
                lineBubbles.Clear();
            }

            // ���C���`�揈��
            if (lineBubbles.Count > 1)
            {
                // ���_��
                lineRenderer.positionCount = lineBubbles.Count;
                // ���C���̃|�W�V����
                for (int i = 0; i < lineBubbles.Count; i++)
                {
                    lineRenderer.SetPosition(i, lineBubbles[i].transform.position);
                }
            }

            /*
             // �^�b�`����
             if(Input.GetMouseButtonUp(0))
             {
                 GameObject hitBubble = GetHitBubble();

                 // �폜���ꂽ�A�C�e�����N���A
                 bubbles.RemoveAll(item => item == null);

                 // ���������蔻�肪�����
                 if (hitBubble)
                 {
                     CheckItems(hitBubble);
                 }
             }
            */
        }
        else
        {
            // �G�̏�Ԃ��U�������Ȃ�
            if (enemy.EnemyCurrentStatus == EnemyManager.EnemyStatus.AttackComplete)
            {
                // �^�[����ύX
                ChangTurn();

                // �G�̏�Ԃ��Ȃ��ɂ���
                enemy.EnemyCurrentStatus = EnemyManager.EnemyStatus.None;
            }
            // �G�̏�Ԃ��Ȃɂ��Ȃ��Ȃ�
            else if (enemy.EnemyCurrentStatus == EnemyManager.EnemyStatus.None)
            {
                Debug.Log(isEnemyTurn);
                // �G�̍U�����J�n
                StartCoroutine(enemy.Attack(isEnemyTurn));
            }
            /*else if(enemy.IsEnemyChange == true)
            {
                enemy.ResetEnemyChange();

                // �G�̏�Ԃ��Ȃ��ɂ���
                enemy.EnemyCurrentStatus = EnemyManager.EnemyStatus.None;

                isPlayerTurn = true;
                isEnemyTurn = false;
            }*/
        }
    }

    // �V�[���̃��[�h��x�点��
    private IEnumerator waitAndLoadScene()
    {
        yield return new WaitForSeconds(1.5f);

        if (isClear)
        {
            // ���U���g���
            SceneManager.LoadScene("Clear");
        }
        else if (isGameOver)
        {
            // ���U���g���
            SceneManager.LoadScene("GameOver");
        }
    }

    // �A�C�e������
    void SpawnItem(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // �F�����_��
            int rnd = Random.Range(0, prefabBubbles.Count);
            // �ꏊ�����_��
            float x = Random.Range(-2.0f, 2.0f);
            float y = Random.Range(-2.0f, 2.0f);

            // �A�C�e������
            bubble =
                Instantiate(prefabBubbles[rnd], new Vector3(x, 3 + y, 0), Quaternion.identity);

            // �����f�[�^�ǉ�
            bubbles.Add(bubble);
        }
    }

    // �����Ɠ����F�̃A�C�e�����폜����
    void DeleteItems(List<GameObject> checkItems)
    {
        // �폜�\���ɒB���Ă��Ȃ�������Ȃɂ����Ȃ�
        if (checkItems.Count < deleteCount) return;

        // �폜���ăX�R�A���Z
        List<GameObject> destroyItems = new List<GameObject>();
        foreach (var item in checkItems)
        {
            // ���Ȃ��̍폜�����A�C�e�����J�E���g
            if (!destroyItems.Contains(item))
            {
                destroyItems.Add(item);
            }

            // �폜
            Destroy(item);
        }

        // ���ۂɍ폜�������������ăX�R�A���Z
        SpawnItem(destroyItems.Count);
        gameScore += destroyItems.Count * 100;

        // �U���͂Ɍq�������� * 30�����Z
        //powerNum += checkItems.Count * 40;

        player.AttackPower(checkItems.Count * 40);

        // �U�����ɕύX
        isDelete = true;

        // �X�R�A�\���X�V
        //textGameScore.text = "" + gameScore;

        // SE�Đ�
        //audioSource.PlayOneShot(seBubble);
    }

    // �����F�̃A�C�e����Ԃ�
    List<GameObject> GetSameItems(GameObject target)
    {
        List<GameObject> ret = new List<GameObject>();

        foreach (var item in bubbles)
        {
            // �A�C�e�����Ȃ��A�����A�C�e���A�Ⴄ�F�A�����������ꍇ�̓X�L�b�v
            if (!item || target == item) continue;

            if (item.GetComponent<SpriteRenderer>().sprite
                != target.GetComponent<SpriteRenderer>().sprite)
            {
                continue;
            }

            float distance =
                Vector2.Distance(target.transform.position, item.transform.position);

            if (distance > 1.1f) continue;

            // �����܂ŗ�����A�C�e���ǉ�
            ret.Add(item);
        }

        return ret;
    }

    // �����Ɠ����F�̃A�C�e����T��
    void CheckItems(GameObject target)
    {
        // ���̃A�C�e���Ɠ����F��ǉ�����
        List<GameObject> checkItems = new List<GameObject>();
        // ������ǉ�
        checkItems.Add(target);

        // �`�F�b�N�ς̃C���f�b�N�X
        int checkIndex = 0;

        // checkItems�̍ő�l�܂Ń��[�v
        while (checkIndex < checkItems.Count)
        {
            // �אڂ��铯���F���擾
            List<GameObject> sameItems = GetSameItems(checkItems[checkIndex]);
            // �`�F�b�N�ς̃C���f�b�N�X��i�߂�
            checkIndex++;

            // �܂��ǉ�����Ă��Ȃ��A�C�e����ǉ�
            foreach (var item in sameItems)
            {
                if (checkItems.Contains(item)) continue;
                checkItems.Add(item);
            }
        }

        // �폜
        DeleteItems(checkItems);
    }

    // �}�E�X�|�W�V�����ɓ����蔻�肪�������o�u����Ԃ�
    GameObject GetHitBubble()
    {
        GameObject ret = null;

        // �X�N���[�����W���烏�[���h���W�ɕϊ�
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit2d = Physics2D.Raycast(worldPoint, Vector2.zero);

        // �����蔻�肠��
        if (hit2d)
        {
            // �摜���ݒ肳��Ă�����o�u���A�C�e���Ɣ��f����
            SpriteRenderer spriteRenderer =
                hit2d.collider.gameObject.GetComponent<SpriteRenderer>();

            if (spriteRenderer)
            {
                ret = hit2d.collider.gameObject;
            }
        }

        return ret;
    }

    // ���g���C�{�^��
    /*public void OnClickRetry()
    {
        SceneManager.LoadScene("SamegamePuzzleScene");
    }*/

    public void Click()
    {
        // �^�[�����I��
        gameTimer = 0;
    }

    // �Q�[���I��
    public void GameEnd()
    {
        isEnd = true;

        StartCoroutine(waitAndLoadScene());
    }

    // �U���I��
    public void EndAttack()
    {
        // �U�����I��点��
        isDelete = false;
        // Attack�{�^���\������Ă��邩
        isAttackBtn = true;
    }

    // ���E���h�J�E���g
    public void CountRound()
    {
        roundCnt++;

        if (isEnd == false)
        {
            uiManager.RoundUpText(roundCnt);
        }
    }

    // �U��^�[���̕ύX
    public void ChangTurn()
    {
        gameTimer = 30;

        if (isPlayerTurn)
        {
            isPlayerTurn = false;
            isEnemyTurn = true;
        }
        else
        {
            isPlayerTurn = true;
            isEnemyTurn = false;
        }

        StartCoroutine(uiManager.ChangeText());
    }

    public IEnumerator WaitChangeTurn()
    {
        yield return new WaitForSeconds(3);

        ChangTurn();
    }

    // �{�^���̕\���E��\��
    public void DisplayAttackBtn(bool isAttack)
    {
        isAttackBtn = isAttack;
    }

    // �{�^����������������Z�b�g
    public void ResetAttackButton()
    {
        isAttackBtn = false;
    }

    public void WinPlayer()
    {
        isClear = true;
    }

    public void LosePlayer()
    {
        isGameOver = true;
    }
}
