using UnityEngine;

public class LaserNew {
    public GameObject laserTarget;
    Material mat1, mat2;
    Material currentMaterial;
    string ignoreTag = "";

    public LaserNew(Vector3 pos, Vector3 dir, Material red, Material green) {
        this.laserTarget = new GameObject();
        this.laserTarget.name = "Laser Beam";
        this.mat1 = red;
        this.mat2 = green;
        this.currentMaterial = this.mat1;
        this.laserTarget.tag = "Laser";
        addLaserSegment(pos, dir, this.mat1, 0);
    }

    private void addLaserSegment(Vector3 origin, Vector3 direction, Material mat, int depth) {
        if (depth > 20) {
            return;
        }
    
        LineRenderer nextSegment = makeLineRenderer(origin, mat);
        Ray ray = new Ray(origin, direction);
        RaycastHit hit; 

        if (closestHit(Physics.RaycastAll(ray), out hit)) {
            ignoreTag = hit.collider.gameObject.tag;
            nextSegment.SetPosition(1, hit.point);
            checkHit(hit, direction, depth + 1);
        } else {
            nextSegment.SetPosition(1, direction * 20 + origin);
        }
    }

    private void checkHit(RaycastHit hitInfo, Vector3 direction, int depth) {
        switch (hitInfo.collider.gameObject.tag) {
            case "Amp":
                this.currentMaterial = mat2;
                addLaserSegment(hitInfo.point, direction, this.currentMaterial, depth);
                break;
            case "Portal1":
                handlePortal("Portal2", depth);
                break;
            case "Portal2":
                handlePortal("Portal1", depth);
                break;
            case "Mirror":
                this.currentMaterial = mat1;
                addLaserSegment(hitInfo.point, hitInfo.normal, this.currentMaterial, depth);
                break;
        }
    }

    private void handlePortal(string tag, int depth) {
        GameObject obj = GameObject.FindWithTag(tag);
        Vector3 origin = obj.transform.position;
        Vector3 direction = obj.transform.up;
        addLaserSegment(origin, direction, this.currentMaterial, depth); 
    }

    private LineRenderer makeLineRenderer(Vector3 origin, Material mat) {
        GameObject child = new GameObject();
        child.transform.parent = this.laserTarget.transform;

        LineRenderer segment = child.AddComponent(typeof(LineRenderer)) as LineRenderer;    
        segment.startWidth = 0.02f;
        segment.endWidth = 0.02f;
        segment.material = mat;
        segment.SetPosition(0, origin);
        return segment;
    }

    private bool closestHit(RaycastHit[] hits, out RaycastHit closest) {
        float min = 9999999;
        bool ok = false;
        closest = new RaycastHit();
        foreach (var hit in hits) {
            if (hit.collider.gameObject.tag == ignoreTag) {
                continue;
            }

            if (hit.distance < min) {
                ok = true;
                min = hit.distance;
                closest = hit;
            }            
        }

        return ok;
    }
}
