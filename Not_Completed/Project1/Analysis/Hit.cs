using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace SpecialMove
{
    public class Hit : MonoBehaviourPun
    {
        int i, j;
        public int cool;
        public int timeRooted; // 경직 시간

        static public bool rooted; // 경직 상태를 판단

        public Vector3 targetPoint;

        void FixedUpdate() // 경직이 풀리게 해주고 cool 감소
        {
            if (timeRooted != 0)
            {
                timeRooted--;
            }
            else
            {
                rooted = false;
            }
            cool--;
        }
        public void OnHitAutoAttack(Quaternion rotation)
        {
            this.transform.rotation = rotation;
            this.transform.Translate(Vector3.right);
            this.transform.rotation = Quaternion.Euler(0, 0, 0);

        }
        public void OnHitSkill1()
        {
            rooted = true;
            timeRooted = 200;
        }
        public void OnHitSkill2(Quaternion rotation)
        {
            if (cool <= 0)
            {
                cool = 1000;
                this.transform.rotation = rotation;
                this.transform.Translate(2, 0, 0);

            }
        }
        public void OnHitSkill3Self(Vector2 position, Quaternion rotation)
        {

            rooted = true;
            timeRooted = 30;
            this.transform.position = position;
            this.transform.rotation = rotation;

        }
        public void OnHitSkill3(Vector2 position, Quaternion rotation)
        {
            rooted = true;
            timeRooted = 30;
            this.transform.position = position;

        }
        public void OnHitSkill4(Vector2 pos)
        {

            this.transform.position = new Vector2(this.transform.position.x + (pos.x - this.transform.position.x) * 0.005f, this.transform.position.y + (pos.y - this.transform.position.y) * 0.005f);

        }
    }
}