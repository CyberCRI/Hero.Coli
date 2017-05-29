    // Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MineExplosion : MonoBehaviour {
    bool fractureToPoint= true;
    int totalMaxFractures= 2;
    float forcePerDivision= 0.5f;
    float minBreakingForce= 0.0f;
    int maxFracturesPerCall= 1;
    float randomOffset= 0.0f;
    Vector3 minFractureSize = new Vector3 (0.5f,0f,0.5f);
    Vector3 grain = Vector3.one;
    float useCollisionDirection= 0.0f;
    bool fractureAtCenter= false;
    bool smartJoints= false;
    float destroyAllAfterTime= 0.0f;
    float destroySmallAfterTime= 0.0f;
    GameObject instantiateOnBreak;
    float totalMassIfStatic= 1.0f;
    private Joint[] joints;


    //replaces all instances or just the first one
    private bool replace<T>(T[] array, T oldElement, T newElement, bool all = true)
    {
        bool found = false;
        for(int i=0;i<array.Length;i++)
        {
            if(EqualityComparer<T>.Default.Equals(array[i],oldElement))
            {
                found = true;
                array[i] = newElement;
                  if(!all)
                {
                    return true;
                }
            }
        }
        return found;
    }

    private bool replaceAll<T>(T[] array, T oldElement, T newElement) {
        return replace(array, oldElement, newElement, true);
    }

    //-------------------------------------------------------------------
    void  Start (){
        if (GetComponent<Rigidbody>()) {
            ArrayList temp = new ArrayList();
            foreach(Joint joint in FindObjectsOfType(typeof(Joint))) {
                if (joint.connectedBody == GetComponent<Rigidbody>()) {
                    temp.Add(joint);
                    joints = (Joint[])temp.ToArray(typeof(Joint));
                }
            }
        }
    }
    //-------------------------------------------------------------------
    void  OnCollisionEnter ( Collision collision  ){

        if(collision.gameObject.tag == Character.playerTag)
        {
            //Debug.DrawLine(transform.position, collision.gameObject.transform.position, Color.red, 1.0ff);
            if(transform.parent && transform.parent.GetComponent<Mine>() != null)
            {
                Vector3 point=collision.contacts[0].point;
                Vector3 vec = collision.relativeVelocity*UsedMass(collision);
          
                FractureAtPoint(point,vec);


                transform.parent.GetComponent<Mine>().detonate();

                collision.gameObject.GetComponent<Character>().kill(CustomDataValue.MINE);
            }
        }
        else if (collision.gameObject.tag == "NPC")
        {
            if (transform.parent && transform.parent.GetComponent<Mine>() != null)
            {
                Vector3 point = collision.contacts[0].point;
                Vector3 vec = collision.relativeVelocity * UsedMass(collision);

                FractureAtPoint(point, vec);


                transform.parent.GetComponent<Mine>().detonate();
                collision.gameObject.GetComponent<iTweenEvent>().enabled = false;
                collision.gameObject.GetComponent<DeathDummy>().startDeath();
            }
        }
    }
    //-------------------------------------------------------------------
    void  FractureAtPoint ( Vector3 hit ,   Vector3 force  ){

        //if (force.magnitude < Mathf.Max(minBreakingForce,forcePerDivision)) {return;}
        //force = new Vector3(0.2ff,0f,0.2ff);
        //force = Vector3.one;
        ///iterations =  Mathf.Min(Mathf.RoundToInt(force.magnitude/forcePerDivision),Mathf.Min(maxFracturesPerCall,totalMaxFractures));
        Vector3 point = transform.worldToLocalMatrix.MultiplyPoint(hit);
        int iterations = 3;
        Fracture(point,force,iterations);
    }
    //-------------------------------------------------------------------
    IEnumerator Fracture ( Vector3 point ,   Vector3 force ,   float iterations  ){
        if (instantiateOnBreak && force.magnitude >= Mathf.Max(minBreakingForce,forcePerDivision)) {
            Instantiate(instantiateOnBreak,transform.position,transform.rotation);
            instantiateOnBreak = null;

        }
        while (iterations > 0) {
            // if we are smaller than our minimum fracture size in any dimension, no more divisions.
            if (totalMaxFractures == 0 || Vector3.Min(gameObject.GetComponent<MeshFilter>().mesh.bounds.size,minFractureSize) != minFractureSize) {
          if (destroySmallAfterTime >= 1) {
                    Destroy(GetComponent<MeshCollider>(),destroySmallAfterTime-1);
                    Destroy(gameObject,destroySmallAfterTime);
                }
                totalMaxFractures = 0;
                yield return null;
            }
            totalMaxFractures -= 1;
            iterations -= 1;
            // define the splitting plane by the user settings.
            if(fractureAtCenter) {
                point=GetComponent<MeshFilter>().mesh.bounds.center;
            }
        Vector3 vec = Vector3.Scale(grain,Random.insideUnitSphere).normalized;
            Vector3 sub = transform.worldToLocalMatrix.MultiplyVector(force.normalized)*useCollisionDirection*Vector3.Dot(transform.worldToLocalMatrix.MultiplyVector(force.normalized),vec);
            Plane plane = new Plane(vec-sub,Vector3.Scale(Random.insideUnitSphere,GetComponent<MeshFilter>().mesh.bounds.size)*randomOffset+point);
            // create the clone
            GameObject newObject = (GameObject) Instantiate(gameObject,transform.position,transform.rotation);
            if (GetComponent<Rigidbody>()) {
                newObject.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;
            }
            // arrays of the verts
            Vector3[] vertsA = gameObject.GetComponent<MeshFilter>().mesh.vertices;
            Vector3[] vertsB = newObject.GetComponent<MeshFilter>().mesh.vertices;
            Vector3 average = Vector3.zero;
            foreach(Vector3 vector in vertsA) {
                average += vector;
            }
            average /= gameObject.GetComponent<MeshFilter>().mesh.vertexCount;
            average -= plane.GetDistanceToPoint(average)*plane.normal;
            //-------------------------------------------------------------------
            int broken = 0;
            // split geometry along plane
            if (fractureToPoint) {
                for (int i=0;i<gameObject.GetComponent<MeshFilter>().mesh.vertexCount;i++) {
                    if (plane.GetSide(vertsA[i])) {
                        vertsA[i] = average;
                        broken += 1;
                    }
                    else {
                        vertsB[i] = average;
                    }
                }
            }
        else {
                for (int i=0;i<gameObject.GetComponent<MeshFilter>().mesh.vertexCount;i++) {
                    if (plane.GetSide(vertsA[i])) {
                        vertsA[i] -= plane.GetDistanceToPoint(vertsA[i])*plane.normal;
                        broken += 1;
                    }
                    else {
                        vertsB[i] -= plane.GetDistanceToPoint(vertsB[i])*plane.normal;
                    }
                }
            }
            // IMPORTANT: redo if we have a problem splitting; without this, we will get a lot of non-manifold meshes, convexhull errors and maybe even crash the game.
            if (broken == 0 || broken == gameObject.GetComponent<MeshFilter>().mesh.vertexCount) {
                totalMaxFractures += 1;
                iterations += 1;
                Destroy(newObject);
                // this yield is here JUST so that when a large amount of objects are being broken, the screen doesn't freeze for a long time. It allows the screen to refresh before we're finnished, but if you don't, it might slow the script down trying to break a loop of bad planes.
                yield return 0;
            }
            // if all's fine, apply the changes to each mesh
            else {
                gameObject.GetComponent<MeshFilter>().mesh.vertices = vertsA;
                newObject.GetComponent<MeshFilter>().mesh.vertices = vertsB;
                gameObject.GetComponent<MeshFilter>().mesh.RecalculateNormals();
                newObject.GetComponent<MeshFilter>().mesh.RecalculateNormals();
                gameObject.GetComponent<MeshFilter>().mesh.RecalculateBounds();
                newObject.GetComponent<MeshFilter>().mesh.RecalculateBounds();
                if (gameObject.GetComponent<MeshCollider>()) {
                    gameObject.GetComponent<MeshCollider>().sharedMesh = gameObject.GetComponent<MeshFilter>().mesh;
                    newObject.GetComponent<MeshCollider>().sharedMesh = newObject.GetComponent<MeshFilter>().mesh;
                }
                // if we weren't using a convexhull, the pieces colliders won't work right. It's best for everyone if we just remove them.
                else {
                    Destroy(GetComponent<Collider>());
                    Destroy(gameObject,1);
                }
                // smartjoints will allow joints to function properly.
                if (smartJoints) {
                    Joint[] jointsb = GetComponents<Joint>();
                    if (0 != jointsb.Length){
                        // Basically, it goes through each joint and sees if the object A or B are closer to the connected body. Whichever is closer keeps the joint.
                        for (int i=0;i<jointsb.Length;i++){
                            if (jointsb[i].connectedBody != null && plane.GetSide(transform.worldToLocalMatrix.MultiplyPoint(jointsb[i].connectedBody.transform.position))) {
                                if (null != jointsb[i].gameObject.GetComponent<MineExplosion>().joints) {
                                    // If we're attached to a fracture object and the new object is closer, switch the connected object's joint variable at the correct index.
                                    replaceAll(
                                        jointsb[i].gameObject.GetComponent<MineExplosion>().joints,
                                        jointsb[i],
                                        newObject.GetComponents<Joint>()[i]);
                                }
                                Destroy(jointsb[i]);
                            }
                            else {
                                Destroy(newObject.GetComponents<Joint>()[i]);
                            }
                        }
                    }
                    // joints contains all joints this object is attached to. It checks if the joint still exists, and if the new object is closer. If so, changes the connection. It then removes the joint from the joints variable at the correct index.
                    if (0 != joints.Length){
                        for (int i=0;i<joints.Length;i++){
                            if (joints[i] && plane.GetSide(transform.worldToLocalMatrix.MultiplyPoint(joints[i].transform.position))) {
                                joints[i].connectedBody = newObject.GetComponent<Rigidbody>();
                                List<Joint> temp = new List<Joint>(joints);
                                temp.RemoveAt(i);
                                temp.CopyTo(joints);
                            }
                            else {
                                List<Joint> temp = new List<Joint>(joints);
                                temp.RemoveAt(i);
                                temp.CopyTo(newObject.GetComponent<MineExplosion>().joints);
                            }
                        }
                    }
                }
                // if we don't have smartJoints, the code is much shorter. destroy all joints.
                else {
                    if (GetComponent<Joint>()) {
                        for (int i=0;i<GetComponents<Joint>().Length;i++){
                            Destroy(GetComponents<Joint>()[i]);
                            Destroy(newObject.GetComponents<Joint>()[i]);
                        }
                    }
                    if (0 != joints.Length) {
                        for (int i=0;i<joints.Length;i++){
                            Destroy(joints[i]);
                        }
                        joints = null;
                    }
                }
                // if the script is attached to a static object, make it dynamic. If not, divide the mass up.
                if (!GetComponent<Rigidbody>()) {
                    gameObject.AddComponent<Rigidbody>();
                    newObject.AddComponent<Rigidbody>();
                    GetComponent<Rigidbody>().mass = totalMassIfStatic;
                    newObject.GetComponent<Rigidbody>().mass = totalMassIfStatic;
                }
                gameObject.GetComponent<Rigidbody>().mass *= 0.5f;
                newObject.GetComponent<Rigidbody>().mass *= 0.5f;
                gameObject.GetComponent<Rigidbody>().centerOfMass = transform.worldToLocalMatrix.MultiplyPoint3x4(gameObject.GetComponent<Collider>().bounds.center);
                newObject.GetComponent<Rigidbody>().centerOfMass = transform.worldToLocalMatrix.MultiplyPoint3x4(newObject.GetComponent<Collider>().bounds.center);

                newObject.GetComponent<MineExplosion>().Fracture(point,force,iterations);

                if (destroyAllAfterTime >= 1) {
                    Destroy(newObject.GetComponent<MeshCollider>(),destroyAllAfterTime-1);
                    Destroy(GetComponent<MeshCollider>(),destroyAllAfterTime-1);
                    Destroy(newObject,destroyAllAfterTime);
                    Destroy(gameObject,destroyAllAfterTime);
                }
                // this yield is here JUST so that when a large amount of objects are being broken, the screen doesn't freeze for a while.
                yield return 0;
            }// if not broken end
        }// while itterations end
        if (totalMaxFractures == 0 || Vector3.Min(gameObject.GetComponent<MeshFilter>().mesh.bounds.size,minFractureSize) != minFractureSize) {
            if (destroySmallAfterTime >= 1) {
                Destroy(GetComponent<MeshCollider>(),destroySmallAfterTime-1);
                Destroy(gameObject,destroySmallAfterTime);
            }
            totalMaxFractures = 0;
        }
    }
    //--------------------------------------------------------------
    private float UsedMass ( Collision collision  ){
        float mass = 1;
        if (collision.rigidbody) {
        if (GetComponent<Rigidbody>()) {
                if (collision.rigidbody.mass < GetComponent<Rigidbody>().mass) {
                    mass = collision.rigidbody.mass;
                }
                else {
                    mass = GetComponent<Rigidbody>().mass;
                }
        }
            else {
                mass = collision.rigidbody.mass;
            }
        }
        else if (GetComponent<Rigidbody>()) {
        mass = GetComponent<Rigidbody>().mass;
        }
        return mass;
    }

    void  deletePieces (){
        if(transform.position.y <=-50)
        Destroy(gameObject);
    }

    void  Update (){

        deletePieces() ;
    }
}
