using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;

namespace SpecialMove
{
    public class Touch : MonoBehaviourPun
    { 
        private GameObject[] arrow = new GameObject[3];
        private GameObject[] cool = new GameObject[6];
        public GameObject Line;
        public GameObject Guide;
        public GameObject Gong;
        public GameObject UltPref;
        public Vector2 leftstart;
        public Vector2 left1;
        public Vector2 leftend;
        public Vector2 rightstart;
        public Vector2 right1;
        public Vector2 rightend;
        public Vector2 arrowPosition;
        public Color color;
        public Sprite sprite;
        public float distanceMove;
        public float distanceCommand;
        public float distanceSkill;
        // leftstart - left1
        public float directionMove;
        // arrowPosition - left1
        public float directionCommand;
        public float directionSkill;
        public float time;
        public float unit;
        public float roundMax;
        public float stackUlt;
        public float[] coolDown = new float[6] { 1, 1, 1, 1, 1, 1 };
        public int i;
        public int j;
        public int finger1;
        public int finger2;
        public int skillCommand;
        public int[] command = new int[3] { 0, 0, 0 };
        public bool round;
        public bool ui;
        public bool adjust;
        public bool startSkill;
        public bool gameStart;
        public Hit hit;
        public int ult;
        void Awake()
        {
            arrow[2] = GameObject.Find("commandThird");
            arrow[1] = GameObject.Find("commandSecond");
            arrow[0] = GameObject.Find("commandFirst");
            cool[0] = GameObject.Find("CoolF");
            cool[1] = GameObject.Find("Cool1");
            cool[2] = GameObject.Find("Cool2");
            cool[3] = GameObject.Find("Cool3");
            cool[4] = GameObject.Find("Cool4");
            cool[5] = GameObject.Find("CoolU");
            Gong = GameObject.Find("GONG");
            Line = GameObject.Find("Line");
            Guide = GameObject.Find("Guide");
        }
        void Start()
        {
            unit = Screen.width * 0.001f; // unit의 크기를 스크린 너비의 0.001 만큼으로 설정
            gameStart = false; // 게임 시작을 지정하는 bool형 gameStart는 false로 지정
            CommandDisplay(); // CommandDisplay 함수로 arrow(커맨드 입력한 거 보여주는 이미지) 최초 회전 값 지정 (default : 0, 90, 0 이러면 안보임)
            j = 20; // j값 20으로 설정
            color = arrow[0].GetComponent<Image>().color; // color 변수에 arrow[0]의 color를 저장
            Guide.SetActive(true); // Guide(조이스틱)를 활성화 시킴
            hit = this.gameObject.GetComponent<Hit>(); // Hit 클래스에 현재 오브젝트가 가지고 있는 컴포넌트 Hit를 넣음
        }
        void FixedUpdate()
        {
            if (Gong == null && !gameStart) // 게임 오브젝트인 GONG(??)이 null이고 (bool)gameStart가 false면
            {
                gameStart = true; // 게임을 시작시킨다.
                SetPosition(); // 오브젝트들의 최초 위치 지정(1P, 2P)
            }
            j--; // j를 -1
            time--; // time을 -1
            if (j == 0)
                j = 20; // j는 20 -> 0 반복
            if (time < 0)
            {
                skillCommand = 0; // skillCommand를 0으로
                command[2] = command[1] = command[0] = 0; // int 배열 command의 요소 값을 전부 0으로
                arrow[0].transform.rotation = Quaternion.Euler(0, 90, 0);
                arrow[1].transform.rotation = Quaternion.Euler(0, 90, 0);
                arrow[2].transform.rotation = Quaternion.Euler(0, 90, 0); // GameObject 배열 arrow의 모든 요소 값의 회전 값 중 y를 90으로 변경

                // Line 94 ~ 96 대신 CommandDisplay 넣었어도 됐을 듯
            }
            directionCommand = Mathf.Rad2Deg * Mathf.Atan2(arrowPosition.y - left1.y, arrowPosition.x - left1.x) + 180;
            directionMove = Mathf.Rad2Deg * Mathf.Atan2(leftstart.y - left1.y, leftstart.x - left1.x) + 180;
            directionSkill = Mathf.Rad2Deg * Mathf.Atan2(rightstart.y - right1.y, rightstart.x - right1.x) + 180;
            // directionCommand, Move, Skill의 값을 Mathf함수를 이용해 대입 
            // Rad2Deg는 상수값. Mathf.Atan2는 y / x를 라디안 값을 반환하는데 그것에 곱하면 육십분법(몇 도)으로 이용 가능
            // Atan2는 tan(r) = y / x 의 역함수로 목표 지점까지의 방향을 구함
            // dir이라는 vector2 변수로 각각의 - 계산들을 합쳐줬으면 가독성이 더 좋았을 듯
            // ex) Vector2 dirLeftstart = leftstart - left1;
            CommandColor(); // arrow의 이미지의 Alpha값을 time / 30f로 설정 ( 1 ~ 0 )
            if (Input.touchCount == 0) // 터치된 손가락이 0개일 경우
                finger1 = finger2 = 2; // finger1, 2의 값을 2로 변경
            else if (Input.touchCount == 1) // 1개일 경우
            {
                if (Input.GetTouch(0).position.x < unit * 500) // GetTouch(0)의 x 위치 값이 unit * 500보다 작다면
                    finger1 = 0; // finger1은 0
                else // 그게 아니면
                    finger1 = 1; // finger1은 1
            }
            else // 2개 이상이면
            {
                if (Input.GetTouch(0).position.x < Input.GetTouch(1).position.x) // 첫 번째 터치 위치의 x값이 두 번째 터치 위치의 x값보다 왼쪽에 있다면
                    finger1 = 0; // finger1은 0
                else // 아니면
                    finger1 = 1; // finger1은 1
            }
            if (Input.touchCount > finger1) // 터치된 손가락의 갯수가 finger1보다 클 경우
                // 위의 조건이 참인 경우는
                // 1. 터치된 손가락이 1개면서 그 위치 값의 x가 unit * 500보다 작은 경우
                // 2. 터치된 손가락이 2개 이상인 경우
            {
                left1 /* 손가락이 움직인 위치를 계속해서 대입 */ = Input.GetTouch(finger1).position; // Vector2 변수 left1에 터치된 손가락(finger1)의 위치 값을 넣음
                switch (Input.GetTouch(finger1).phase) // 터치된 손가락(인덱스)의 페이즈를 검사함(터치됐을 때, 움직일 때, 움직이지 않을 때, 손가락을 떼었을 때)
                {
                    case TouchPhase.Began: // 손가락이 터치되면
                        leftstart = left1; // Vector2 변수 leftstart에 left1(현재 터치된 손가락의 위치값)을 대입
                        arrowPosition = leftstart; // Vector2 변수 arrowPosition에 leftStart를 대입
                        distanceCommand = Vector2.Distance(arrowPosition, left1); // float 변수 distanceCommand에 arrowPosition과 left1 사이의 거리를 대입
                        distanceMove = Vector2.Distance(leftstart, left1); // float 변수 distanceMove에 leftstart와 left1 사이의 거리를 대입
                        RefreshMovePoint(); // leftstart지점의 설정과 Line의 방향, fillAmount 설정
                        MoveCharacter(); // distanceMove를 이용해 캐릭터의 움직임을 담당
                        Commands(); // 커맨드(좌측 하단의 화살표)의 방향과 스킬 커맨드 값 설정
                        Guide.SetActive(false); // 조이스틱 비활성화
                        Line.SetActive(true); // Line(방향선?) 활성화
                        break;
                    case TouchPhase.Moved: // 터치된 손가락이 움직이면
                        distanceCommand = Vector2.Distance(arrowPosition, left1);
                        distanceMove = Vector2.Distance(leftstart, left1);
                        RefreshMovePoint();
                        MoveCharacter();
                        Commands();
                        break;
                    case TouchPhase.Stationary: // 터치된 손가락이 가만히 있으면
                        distanceCommand = Vector2.Distance(arrowPosition, left1);
                        distanceMove = Vector2.Distance(leftstart, left1);
                        RefreshMovePoint();
                        MoveCharacter();
                        Commands();
                        break;
                        // TouchPhase.Moved ~ Stationary까지 동일
                    case TouchPhase.Ended: // 손가락을 떼면
                        Line.SetActive(false); // Line(방향선) 비활성화
                        Guide.SetActive(true); // 조이스틱 활성화
                        leftend = left1; // leftend(끝나는 지점)에 left1(마지막 지점) 대입
                        time = 30; // time에 30 대입
                        // 이 time이 0이 되면 스킬 커맨드, 커맨드(화살표)가 초기화(스킬 커맨드는 0, 커맨드(화살표)는 0, 90, 0으로 설정)된다.
                        break;
                }
            }
        }
        void Update()
        {
            if (finger1 == 0)
                finger2 = 1;
            else
                finger2 = 0;
            // 손가락이 최소 하나라도 닿아 있어야 아래 if 조건이 성립
            if (Input.touchCount > finger2) // 터치된 손가락 갯수가 finger2보다 크면
            {
                right1 = Input.GetTouch(finger2).position; // right1에, 터치된 손가락 중 인덱스가 finger2인 요소의 위치 값 대입
                switch (Input.GetTouch(finger2).phase)
                {
                    case TouchPhase.Began:
                        rightstart = right1; // rightstart에 right1 대입
                        roundMax = 0; // roundMax를 0으로 변경
                        round = false; // round를 false로 변경
                        // round, roundMax는 아직까지 용도불명
                        break;
                    case TouchPhase.Moved:
                        distanceSkill = Vector2.Distance(rightstart, right1); // 오른쪽 손의 시작 지점과 마지막 지점 사이의 거리를 대입
                        // 왼쪽 손으로 이동하면서 커맨드를 입력하고 오른쪽 손으로 스킬의 방향을 지정하는건가?
                        UltCheck(); // 궁극기 체크 아직 용도 불명
                        break;
                    case TouchPhase.Stationary:
                        distanceSkill = Vector2.Distance(rightstart, right1);
                        UltCheck();
                        break;
                    case TouchPhase.Ended:
                        rightend = right1; // 터치가 끝난 지점에 마지막 터치 지점 대입
                        SkillCheck(); // 스킬인지 체크
                        break;
                }
            }
            switch (SkillGetter(skillCommand)) // 몇 번째 스킬이냐에 따라 게임 좌측상단에 있는 직사각형 스킬 칸의 불이 들어오거나 꺼짐
            {
                case "1":
                    cool[1].GetComponent<Image>().color = Color.yellow;
                    cool[2].GetComponent<Image>().color = Color.white;
                    cool[3].GetComponent<Image>().color = Color.white;
                    cool[4].GetComponent<Image>().color = Color.white;
                    break;
                case "2":
                    cool[1].GetComponent<Image>().color = Color.white;
                    cool[2].GetComponent<Image>().color = Color.yellow;
                    cool[3].GetComponent<Image>().color = Color.white;
                    cool[4].GetComponent<Image>().color = Color.white;
                    break;
                case "3":
                    cool[1].GetComponent<Image>().color = Color.white;
                    cool[2].GetComponent<Image>().color = Color.white;
                    cool[3].GetComponent<Image>().color = Color.yellow;
                    cool[4].GetComponent<Image>().color = Color.white;
                    break;
                case "4":
                    cool[1].GetComponent<Image>().color = Color.white;
                    cool[2].GetComponent<Image>().color = Color.white;
                    cool[3].GetComponent<Image>().color = Color.white;
                    cool[4].GetComponent<Image>().color = Color.yellow;
                    break;
                default:
                    cool[1].GetComponent<Image>().color = Color.white;
                    cool[2].GetComponent<Image>().color = Color.white;
                    cool[3].GetComponent<Image>().color = Color.white;
                    cool[4].GetComponent<Image>().color = Color.white;
                    break;
            }
        }
        void LateUpdate()
        {
            if (gameStart)
            {
                // 각 스킬당 쿨다운이 1이상이면 사용 준비가 된 스킬들. 쿨타임이 남았을 때는 시간을 표시한다.
                if (coolDown[1] < 1)
                {
                    coolDown[1] += 0.004167f;
                    cool[1].transform.GetChild(0).GetComponent<Image>().color = Color.black;
                    cool[1].transform.GetChild(1).GetComponent<Text>().text = (4 - coolDown[1] * 4) < 1 ? $"{4 - coolDown[1] * 4:F1}" : $"{4 - coolDown[1] * 4:F0}";
                }
                else
                {
                    cool[1].transform.GetChild(0).GetComponent<Image>().color = Color.white;
                    cool[1].transform.GetChild(1).GetComponent<Text>().text = "";
                }
                if (coolDown[2] < 1)
                {
                    coolDown[2] += 0.002778f;
                    cool[2].transform.GetChild(0).GetComponent<Image>().color = Color.black;
                    cool[2].transform.GetChild(1).GetComponent<Text>().text = (6 - coolDown[2] * 6) < 1 ? $"{6 - coolDown[2] * 6:F1}" : $"{6 - coolDown[2] * 6:F0}";
                }
                else
                {
                    cool[2].transform.GetChild(0).GetComponent<Image>().color = Color.white;
                    cool[2].transform.GetChild(1).GetComponent<Text>().text = "";
                }
                if (coolDown[3] < 1)
                {
                    coolDown[3] += 0.002083f;
                    cool[3].transform.GetChild(0).GetComponent<Image>().color = Color.black;
                    cool[3].transform.GetChild(1).GetComponent<Text>().text = (8 - coolDown[3] * 8) < 1 ? $"{8 - coolDown[3] * 8:F1}" : $"{8 - coolDown[3] * 8:F0}";
                }
                else
                {
                    cool[3].transform.GetChild(0).GetComponent<Image>().color = Color.white;
                    cool[3].transform.GetChild(1).GetComponent<Text>().text = "";
                }
                if (coolDown[4] < 1)
                {
                    coolDown[4] += 0.001667f;
                    cool[4].transform.GetChild(0).GetComponent<Image>().color = Color.black;
                    cool[4].transform.GetChild(1).GetComponent<Text>().text = (10 - coolDown[4] * 10) < 1 ? $"{10 - coolDown[4] * 10:F1}" : $"{10 - coolDown[4] * 10:F0}";

                }
                else
                {
                    cool[4].transform.GetChild(0).GetComponent<Image>().color = Color.white;
                    cool[4].transform.GetChild(1).GetComponent<Text>().text = "";
                }
                // coolDown[5]는 ult로 추정.
                coolDown[5] += 0.003333f;
                // 각각의 쿨타임이 돌 때는 검은색으로 감싸져있는 베일을 걷어내는 듯한 연출을 준다.
                cool[1].GetComponent<Image>().fillAmount = coolDown[1];
                cool[2].GetComponent<Image>().fillAmount = coolDown[2];
                cool[3].GetComponent<Image>().fillAmount = coolDown[3];
                cool[4].GetComponent<Image>().fillAmount = coolDown[4];
                // 궁극기의 쿨다운은 cool[0] 게이지로, 스택의 정도는 cool[5] 게이지로 표현
                cool[0].GetComponent<Slider>().value = coolDown[5];
                cool[5].GetComponent<Slider>().value = stackUlt;
                for (i = 0; i < Input.touchCount; i++)
                {
                    if (EventSystem.current.IsPointerOverGameObject(i))
                        // 손가락이 UI를 클릭했을 경우
                    {
                        ui = true;
                        break;
                    }
                    else
                        ui = false;
                }
                if (startSkill) // 스킬의 준비가 끝났다면
                {
                    skillCommand = command[2] * 100 + command[1] * 10 + command[0];

                    if (coolDown[int.Parse(SkillGetter(skillCommand))] >= 1) // 해당 스킬의 쿨다운이 1 이상이라면
                    {
                        CoolDownReset(int.Parse(SkillGetter(skillCommand))); // 해당 스킬의 쿨다운을 리셋 시키고
                        stackUlt += 0.201f; // ult스택을 추가
                        switch (SkillGetter(skillCommand))
                        {
                            case "1":
                                this.gameObject.GetComponent<PlayerRPC>().Skill11(this.transform.position, Quaternion.Euler(0, 0, directionSkill), PhotonNetwork.Time);
                                break;
                            case "2":
                                this.gameObject.GetComponent<PlayerRPC>().Skill22(this.transform.position, Quaternion.Euler(0, 0, directionSkill), PhotonNetwork.Time);
                                break;
                            case "3":
                                this.gameObject.GetComponent<PlayerRPC>().Skill33(this.transform.position, Quaternion.Euler(0, 0, directionSkill), PhotonNetwork.Time);
                                break;
                            case "4":
                                this.gameObject.GetComponent<PlayerRPC>().Skill44(this.transform.position, Quaternion.Euler(0, 0, directionSkill), PhotonNetwork.Time);
                                break;
                        }
                    }
                    else
                    {
                        this.gameObject.GetComponent<PlayerRPC>().AutoAttack(this.transform.position, Quaternion.Euler(0, 0, directionSkill), PhotonNetwork.Time);
                    }
                    // PlayerRPC 클래스로 정보 전달
                }
                startSkill = false;
            }
        }
        void SetPosition() // 각 대전자의 위치 조정
        {
            if (PhotonNetwork.IsMasterClient) // 포톤네트워크 사용.마스터 클라이언트라면 (1P라면)
            {
                this.gameObject.transform.position = new Vector2(1.5f, 2.5f); // 해당 오브젝트의 위치를 1.5f, 2.5f로 옮김
            }
            else
            {
                this.gameObject.transform.position = new Vector2(6.5f, -2.5f); // 해당 오브젝트의 위치를 6.5f, -2.5f로 옮김
            }
        }
        void MoveCharacter()
        {
            if (!Hit.rooted) // rooted는 공격 당했을 경우의 경직이라고 생각하면 될 듯. 스킬에 피격될 경우에 rooted가 true가 되고 스킬마다의 timerooted만큼 경직.
                            // timerooted가 끝나면 rooted가 false로 변경되면서 움직일 수 있음
            {
                this.transform.rotation = Quaternion.Euler(0, 0, directionMove); // 해당 오브젝트의 회전 값을 directionMove으로 회전
                this.transform.GetChild(0).transform.rotation = Quaternion.Euler(0, 0, 0); // 해당 오브젝트의 자식 오브젝트 중 첫 번째 인덱스에 있는 자식의 회전 값을 0, 0, 0으로 회전)
                if (distanceMove < unit * 20) // distanceMove의 크기가 unit * 20보다 작을 경우
                    this.transform.Translate(transform.right * 0.002f, Space.World); // 월드 기준으로 transfomr.right(x축)에 0.002만큼 더해줌(이동)
                else if (distanceMove < unit * 245) // distanceMove의 크기가 unit * 245보다 작을 경우
                    this.transform.Translate(transform.right * 0.012f, Space.World); // 0.012f만큼 더해줌(10배)
                else
                    this.transform.Translate(transform.right * 0.015f, Space.World); // 이외의 distanceMove에서는 0.015f만큼 더해줌(최고속도인 듯)
            }
        }
        void RefreshMovePoint() // 말 그대로 무브 포인트를 새롭게 지정. leftstart지점의 설정과 Line의 방향, fillAmount 설정
        {
            Line.transform.position = leftstart; // Line(점선)의 시작 지점을 leftstart(터치된 지점)로 옮김
            Line.transform.rotation = Quaternion.Euler(0, 0, directionMove); // Line의 회전 값을 directionMove 방향으로 회전(directionMove는 터치가 시작된 지점에서 터치하고 있는 지점을 바라보는 방향);
            Line.gameObject.GetComponent<Image>().fillAmount = distanceMove * 0.001f; // fillAmount를 이용해서 거리가 멀어질수록 점선이 늘거나 줄어드는 것처럼 보이도록 함
            if (distanceMove > 250) // distanceMove의 크기가 250보다 크다면
            {
                while (Vector2.Distance(leftstart, left1) > 250) // 터치의 시작 지점과 터치하고 있는 지점 사이의 거리가 250보다 크다면
                {
                    leftstart.x += Mathf.Cos(Mathf.Deg2Rad * directionMove); // Mathf.Cos은 좌우운동을 함. -1 ~ 1.
                    leftstart.y += Mathf.Sin(Mathf.Deg2Rad * directionMove); // Mathf.Sin은 상하운도을 함. -1 ~ 1.
                    // leftstart(시작점)의 x, y값을 변경시킴
                    // Mathf.Cos, Sin을 이용하면 이동방향이 이상해져야 하지 않나??
                }
            }
        }
        void UltCheck() // 궁극기 체크?
        {
            if (roundMax < distanceSkill) // roundMax가 원의 끝을 말하는 건가?
            {
                roundMax = distanceSkill;
            }
            else if (distanceSkill > 150 && round == false && distanceSkill < roundMax * 0.7f)
                // distanceSkill이 150보다 크고 round는 false이며 distanceSkill이 roundMax * 0.7f보다 작을 때
            {
                round = true; // round는 참이 된다.
            }
        }
        void SkillCheck() // 스킬 사용을 하는지 안하는지 체크
        {
            roundMax = 0;
            if (round && stackUlt >= 1) // round가 참이고 궁극기 스택이 1 이상이면
            {
                round = false; // round는 다시 false
                Hit.rooted = false; // 경직도 false
                //       UltPref = PhotonNetwork.Instantiate("Ult", Vector3.zero, Quaternion.identity);
                //      UltPref.GetComponent<Ult>().ult = 300;
            }
            else // 아니라면
            {
                if (distanceSkill < unit * 20)
                {
                    if (coolDown[5] >= 1)
                    {
                        CoolDownReset(5);
                        if (Vector2.Distance(Camera.main.WorldToScreenPoint(this.transform.position), rightstart) < unit * 200)
                        {
                            this.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(rightstart).x, Camera.main.ScreenToWorldPoint(rightstart).y, 0);
                        }
                        else
                        {
                            this.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(Camera.main.WorldToScreenPoint(this.transform.position).y - rightstart.y, Camera.main.WorldToScreenPoint(this.transform.position).x - rightstart.x) + 180);
                            this.transform.Translate(transform.right * 3, Space.World);
                            this.transform.GetChild(0).transform.rotation = Quaternion.Euler(0, 0, 0);
                        }
                    }
                }
                else
                {
                    startSkill = true;
                }
            }
        }
        void CoolDownReset(int a) // 해당 스킬의 쿨타임 리셋
        {
            coolDown[a] = 0;
        }
        string SkillGetter(int a) // 스킬을 가져옴
        {
            for (i = 4; i != 0; i--)
            {
                if (Delivery.joyStick[i] == a)
                    break;
            }
            return Delivery.skillName[i];
        }
        void Commands() // 좌측 하단에 생기는 커맨드 입력 칸에 생기는 화살표의 방향 설정
        {
            if ((directionCommand < 30 && directionCommand >= 0) || (directionCommand > 330 && directionCommand <= 360))
                // 1. directionCommand의 값이 30보다 작고 0보다 크거나 같을 때 아니면 330보다 크고 360보다 작거나 같을 때
            {
                if (command[0] == 1) // 1-1. 커맨드의 첫 번째 요소가 1이면
                {
                    if (j == 5) // 1-1-1. j가 5면
                    {
                        arrowPosition = left1; // arrowPostion에 left1(터치되고 있는 지점)을 대입
                    }
                }
                else // 1-2. 아니면
                {
                    if (distanceCommand > unit * 50) // 1-2-1. distanceCommand의 값이 unit * 50보다 크면
                    {
                        command[2] = command[1];
                        command[1] = command[0];
                        command[0] = 1;
                        CommandDisplay();
                        // 인덱스의 요소 값들을 한 칸씩 뒤로 밀고 첫 번째 요소에 1(오른쪽) 대입
                        // 이후 command에 따른 arrow(커맨드 입력에 따라 화살표의 방향을 표시)의 회전 값 지정
                    }
                }
            }
            else if (directionCommand < 210 && directionCommand > 150)
                // 2. directionCommand의 값이 210보다 작고 150보다 크면
            {
                if (command[0] == 2) // 2-1. command의 첫 번째 인덱스의 요소 값이 2라면
                {
                    if (j == 5) // 2-1-1. j가 5라면
                        // j가 프레임 역할을 하는 듯?
                    {
                        arrowPosition = left1; // arrowPosition에 left1 대입
                    }
                }
                else // 2-2. 아니면
                {
                    if (distanceCommand > unit * 50) // 2-2-1. distanceCommand의 값이 unit * 50보다 크면
                    {
                        command[2] = command[1];
                        command[1] = command[0];
                        command[0] = 2;
                        CommandDisplay();
                        // 인덱스의 요소 값들을 한 칸씩 뒤로 밀고 첫 번째 요소에 2(왼쪽) 대입
                        // 이후 command에 따른 arrow(커맨드 입력에 따라 화살표의 방향을 표시)의 회전 값 지정
                    }
                }
            }
            else if (directionCommand < 120 && directionCommand > 60)
                // 3. directionCommand의 값이 120보다 작고 60보다 크면
            {
                if (command[0] == 3) // 3-1. command의 첫 번째 인덱스의 요소 값이 3이면
                {
                    if (j == 5) // 3-1-1. j가 5면
                    {
                        arrowPosition = left1; // arrowPosition에 left1 대입
                    }
                }
                else // 3-2. 아니면
                {
                    if (distanceCommand > unit * 50) // 3-2-1. distanceCommand의 값이 unit * 50보다 크면
                    {
                        command[2] = command[1];
                        command[1] = command[0];
                        command[0] = 3;
                        CommandDisplay();
                        // 인덱스의 요소 값들을 한 칸씩 뒤로 밀고 첫 번째 요소에 3(위) 대입
                        // 이후 command에 따른 arrow(커맨드 입력에 따라 화살표의 방향을 표시)의 회전 값 지정
                    }
                }
            }
            else if (directionCommand < 300 && directionCommand > 240)
                // 4. directionCommand의 값이 300보다 작고 240보다 크면
            {
                if (command[0] == 4) // 4-1. command의 첫 번째 인덱스 요소 값이 4라면
                {
                    if (j == 5) // 4-1-1. j가 5라면
                    {
                        arrowPosition = left1; // arrowPosition에 left1 대입
                    }
                }
                else // 4-2. 아니라면
                {
                    if (distanceCommand > unit * 50) // 4-2-1. ditanceCommand의 값이 unit * 50보다 크다면
                    {
                        command[2] = command[1];
                        command[1] = command[0];
                        command[0] = 4;
                        CommandDisplay();
                        // 인덱스의 요소 값들을 한 칸씩 뒤로 밀고 첫 번째 요소에 4(아래) 대입
                        // 이후 command에 따른 arrow(커맨드 입력에 따라 화살표의 방향을 표시)의 회전 값 지정
                    }
                }
            }
            // directionCommand의 값은 arrowPosition이 left1을 바라보는 방향 값이다. 오른쪽으로 터치를 옮기면 오른쪽으로 화살표가 생성됨( command[index] = 1 )

            skillCommand = command[2] * 100 + command[1] * 10 + command[0];
            // skillCommand에 100의 자리에 세 번째 인덱스, 10의 자리에 두 번째 인덱스, 1의 자리에 첫 번째 인덱스를 넣음
            // 이 숫자를 이용해서 100, 10, 1의 자리에 따라 스킬을 지정하는 듯
        }
        void CommandDisplay() // Command에 따른 Arrow의 회전 값 지정
        {
            for (i = 0; i < 3; i++)
            {
                switch (command[i])
                {
                    case 1:
                        arrow[i].transform.rotation = Quaternion.Euler(0, 0, 0); // 오른쪽
                        break;
                    case 2:
                        arrow[i].transform.rotation = Quaternion.Euler(0, 0, 180); // 왼쪽
                        break;
                    case 3:
                        arrow[i].transform.rotation = Quaternion.Euler(0, 0, 90); // 위
                        break;
                    case 4:
                        arrow[i].transform.rotation = Quaternion.Euler(0, 0, 270); // 아래
                        break;
                    default:
                        arrow[i].transform.rotation = Quaternion.Euler(0, 90, 0); // 안보임
                        break;
                }
            }
            time = 300;
        }
        void CommandColor() // 커맨드 화살표를 시간에 따라 점점 옅어지게 만듦
        {
            arrow[2].GetComponent<Image>().color = new Color(color.r, color.g, color.b, time / 30f);
            arrow[1].GetComponent<Image>().color = new Color(color.r, color.g, color.b, time / 30f);
            arrow[0].GetComponent<Image>().color = new Color(color.r, color.g, color.b, time / 30f);
        }
    }
}