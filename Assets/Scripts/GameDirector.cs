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
    [SerializeField] float gameTimer;
    // �t�B�[���h�̃A�C�e������
    [SerializeField] int fieldItemCountMax;
    // �폜�ł���A�C�e����
    [SerializeField] int deleteCount;
    // �R���{�J�E���g
    [SerializeField] int comboCount;

    // UI
    //[SerializeField] TextMeshProUGUI textGameScore;
    [SerializeField] TextMeshProUGUI textGameTimer;
    [SerializeField] TextMeshProUGUI textComboCounter;
    [SerializeField] TextMeshProUGUI combo;
    [SerializeField] TextMeshProUGUI power;
    [SerializeField] TextMeshProUGUI clear;
    //[SerializeField] GameObject gameResult;
    [SerializeField] GameObject attackButton;

    Enemy enemy;
    GameObject obj;

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
    public bool isEnemyTurn;
    // �I���������ǂ���
    public bool isEnd;
    // �o�߃^�[��
    public int turn = 0;
    // �v���C���[��HP
    public int playerHp = 500;
    // �U����
    int powerNum = 0;
    // ��������
    bool isDelete = false;

    // Start is called before the first frame update
    void Start()
    {
        // Enemy�X�N���v�g�̎擾
        obj = GameObject.Find("Enemy");
        enemy = obj.GetComponent<Enemy>();

        // Audio
        audioSource = GetComponent<AudioSource>();

        // �S�A�C�e��
        bubbles = new List<GameObject>();

        // �c���c����
        lineBubbles = new List<GameObject>();
        // �A�������A�C�e����̃��C��
        lineRenderer = GetComponent<LineRenderer>();

        // ���U���g��ʔ�\��
        //gameResult.SetActive(false);

        // �e�L�X�g��\��
        power.enabled = false;
        clear.enabled = false;

        isPlayerTurn = true;
        isEnemyTurn = false;

        isEnd = false;

        // �U���{�^�����\��
        attackButton.SetActive(false);

        // �A�C�e������
        SpawnItem(fieldItemCountMax);
    }

    // Update is called once per frame
    public void Update()
    {
        // isEnd��true�̂Ƃ�
        if (isEnd)
        {
            // ���U���g��ʕ\��
            //gameResult.SetActive(true);

            // �N���A�e�L�X�g��\��
            clear.enabled = true;
            Debug.Log("���肠");

            // Update�ɓ���Ȃ��悤�ɂ���
            enabled = false;

            // ���̎��_��Update���甲����
            return;
        }
        else if (isEnemyTurn == false)
        {// isEnd��false�E�G�̃^�[������Ȃ��Ƃ�

            // �Q�[���^�C�}�[�X�V����
            gameTimer -= Time.deltaTime;
            textGameTimer.text = "" + (int)gameTimer;

            // �Q�[���I��
            if (0 > gameTimer)
            {
                // �G�̗̑͂����炷
                enemy.hp -= powerNum;
                Debug.Log("�G��HP�F" + enemy.hp);

                textGameTimer.text = "" + 0;

                isDelete = false;

                isEnemyTurn = true;

                enemy.Attack(isEnemyTurn);

                // �^�[���������Z
                turn++;

                // �^�[������2�ȉ���������
                if (turn <= 2)
                {
                    // �^�C�}�[��߂�
                    gameTimer = 30;
                    textGameTimer.text = "" + (int)gameTimer;

                    // �U���͂̐��l�����ɖ߂�
                    powerNum = 0;
                    power.text = "" + 0;

                    // �_���[�W�����\��
                    power.enabled = false;
                    // �U���{�^�����\��
                    attackButton.SetActive(false);
                }
                else
                {
                    textGameTimer.text = "" + 0;

                    // ���U���g��ʕ\��
                    //gameResult.SetActive(true);

                    // Update�ɓ���Ȃ��悤�ɂ���
                    enabled = false;

                    // ���̎��_��Update���甲����
                    return;
                }
            }
        }
        else
        {
            return;
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
            GameObject bubble =
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

        Debug.Log(checkItems.Count);

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

        // �_���[�W����\��
        power.enabled = true;

        // �U���͂Ɍq�������� * 30�����Z
        powerNum += checkItems.Count * 40;
        power.text = "" + (int)powerNum;

        isDelete = true;

        if(isDelete)
        {
            attackButton.SetActive(true);
        }

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
}
