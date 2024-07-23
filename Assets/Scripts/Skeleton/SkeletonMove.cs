using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

enum SkeletonStatus{
    TorchSabotage,
    PlayerAttack,
    NONE
}

public class SkeletonMove : MonoBehaviour
{
    static Dictionary<SkeletonStatus, HashSet<SkeletonMove>> skeletonMoves = new Dictionary<SkeletonStatus, HashSet<SkeletonMove>>();
    SkeletonStatus status;
    Orientation orientation;
    Animator animator;
    SpriteRenderer spriteRenderer;
    Unit unit;
    Rigidbody2D rb;

    private float moveSpeed;
    private GameObject player;
    private Vector3 playerPos;

    private Vector3 skeletonPos;

    private List<Node> path;
    private int currentPathIndex;
    private MapPath mapPath;
    private Tilemap ground;

    private Vector3 lastPosition;
    private float stuckTime;

    private List<Transform> torches;
    int currentTorch = 0;

    private void Awake() {
        if (skeletonMoves.ContainsKey(SkeletonStatus.TorchSabotage)==false){
            skeletonMoves.Add(SkeletonStatus.TorchSabotage, new HashSet<SkeletonMove>());
        }
        if (skeletonMoves.ContainsKey(SkeletonStatus.PlayerAttack)==false){
            skeletonMoves.Add(SkeletonStatus.PlayerAttack, new HashSet<SkeletonMove>());
        }
    }

    void Start()
    {

        void DecideTorchSabotage(){
            if (status!=SkeletonStatus.NONE){
                return;
            }

            // count available torches
            int availableTorches = 0;
            foreach (var torch in torches){
                if (torch.gameObject.GetComponent<Torch>().sabotaged==false){
                    availableTorches++;
                }
            }
            if (availableTorches>=1&&skeletonMoves[SkeletonStatus.TorchSabotage].Count==0){
                // decide this will be torch sabotage
                skeletonMoves[SkeletonStatus.TorchSabotage].Add(this);
                status = SkeletonStatus.TorchSabotage;
            }
            else{
                skeletonMoves[SkeletonStatus.PlayerAttack].Add(this);
                status = SkeletonStatus.PlayerAttack;
            }
        }
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        unit = GetComponent<Unit>();
        rb = GetComponent<Rigidbody2D>();

        torches = new List<Transform>();
        var torchObjects =  GameObject.FindGameObjectsWithTag("Torch").ToList();

        foreach (var torch in torchObjects)
        {
            Transform torchTransform = torch.transform;
            torches.Add(torchTransform);
        }

        mapPath = MapPath.instance;

        player = GameObject.FindGameObjectWithTag("Player");

        skeletonPos = rb.position; // Use rb.position instead of transform.position
        orientation = Orientation.None;

        ground = GameObject.Find("Ground").GetComponent<Tilemap>();

        lastPosition = rb.position;
        stuckTime = 0f;

        status = SkeletonStatus.NONE;
        
        DecideTorchSabotage();
    }

    void Update()
    {

        moveSpeed = MapManager.instance.GetWalkingSpeed(rb.position) * 4f;
        Vector3 target;

        if (status==SkeletonStatus.PlayerAttack){
            Vector3 currentPlayerPos = player.GetComponent<Rigidbody2D>().position;

            if (playerPos == null || Vector3.Distance(currentPlayerPos, playerPos) > 0.01f) // if player has moved
            {
                playerPos = currentPlayerPos;

                // navigate to player
                FindPathTo(playerPos);
            }

            target = playerPos; // set target
        }
        else{
            if (path == null) // no path to torch found
            {
                // navigate to player
                FindPathTo(torches[currentTorch].position);
            }

            target = torches[currentTorch].position;
        }
        if (status==SkeletonStatus.TorchSabotage){
            while (torches[currentTorch].GetComponent<Torch>().sabotaged)
            { // current torch has been sabotaged => not finding it anymore and find the next one
                currentTorch++;
                if (currentTorch>=torches.Count){
                    skeletonMoves[SkeletonStatus.TorchSabotage].Remove(this);
                    skeletonMoves[SkeletonStatus.PlayerAttack].Add(this);
                    status = SkeletonStatus.PlayerAttack; // switch to attack player
                    currentTorch = 0;
                    return;
                }

            }
        }

        MoveAlongPath(target);

        if (TimeManage.instance.IsDay()){
            DieWhenDay();
        }

        CheckIfStuck();

        // Debug.Log(target);
    }

    private void DieWhenDay(){
        unit.Die(); // when day, a skeleton automatically dies
    }

    private void FindPathTo(Vector3 pos)
    {
        path = AStarPathfinding(rb.position, pos);

        if (path != null && path.Count > 0)
        {
            currentPathIndex = 0;
        }
    }

    private void MoveAlongPath(Vector3 target)
    {
        if (path == null || currentPathIndex >= path.Count)
            return;

        string strPath = "";
        foreach (var node in path){
            strPath += node;
        }

        Vector3 targetPosition;

        // reached goal then move toward nearer to the target
        if (currentPathIndex == path.Count - 1)
        {
            targetPosition = target;

            if (status==SkeletonStatus.TorchSabotage&&Vector3.Distance(rb.position, target) <= 1f)
            {
                SabotageTorch(); // Attack the torch when close
                return;
            }
            else{
                rb.MovePosition(Vector3.MoveTowards(rb.position, target, 0.5f));
            }
        }
        else
        { // move near target
            targetPosition = ground.CellToWorld(path[currentPathIndex].Position) + new Vector3(0.5f, 0.5f, 0);
            if (Vector3.Distance(rb.position, targetPosition) < 0.1f)
            {
                currentPathIndex++;
                if (currentPathIndex>=0){
                    path[currentPathIndex-1].Occupied = false; // mark the previous node as not occupied anymore
                }
                path[currentPathIndex].Occupied = true;
            }
        }

        Vector3 direction = (targetPosition - (Vector3)rb.position).normalized;
        rb.MovePosition((Vector3)rb.position + direction * moveSpeed * Time.deltaTime);

        UpdateOrientation();
        skeletonPos = rb.position; // Use rb.position instead of transform.position
    }

    private void UpdateOrientation()
    {
        Vector3 diff = (Vector3)rb.position - skeletonPos; // compare current pos with previous pos
        if (Mathf.Abs(diff.y) >= Mathf.Abs(diff.x))
        {
            if (diff.y > 0 && orientation != Orientation.UP)
            {
                SetOrientation(Orientation.UP);
            }
            else if (diff.y < 0 && orientation != Orientation.DOWN)
            {
                SetOrientation(Orientation.DOWN);
            }

        }
        else
        {
            if (diff.x > 0 && orientation != Orientation.RIGHT)
            {
                SetOrientation(Orientation.RIGHT);
            }
            else if (diff.x < 0 && orientation != Orientation.LEFT)
            {
                SetOrientation(Orientation.LEFT);
            }
        }

    }

    private void SetOrientation(Orientation newOrientation)
    {
        if (orientation == Orientation.None || orientation != newOrientation)
        {
            orientation = newOrientation;
            animator.SetBool("up", orientation == Orientation.UP);
            animator.SetBool("down", orientation == Orientation.DOWN);
            animator.SetBool("horizontal", orientation == Orientation.LEFT || orientation == Orientation.RIGHT);
            spriteRenderer.flipX = orientation == Orientation.LEFT;
        }
    }

    private bool isAttacking = false;
    private float nextAttackTime = 0f; 
    public float attackCooldown = 2.5f; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")&&other.gameObject.GetComponent<PlayerAttack>().isAttacking)
        { // dodge player
            DodgePlayer(other.gameObject.transform);
        }
        else if (other.gameObject.CompareTag("Torch"))
        {
            Debug.Log("Attacked torch");
            SabotageTorch();
        }
    }

    private void DodgePlayer(Transform player){
        Vector2 otherPosition = player.position;
        Vector2 directionAwayFromOther = (rb.position - otherPosition).normalized;

        // opposite direction of player
        rb.MovePosition((Vector3)rb.position + (Vector3)directionAwayFromOther * 0.1f * Time.deltaTime);
        UpdateOrientation();
        skeletonPos = rb.position; 
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isAttacking && other.gameObject.CompareTag("Player") && Time.time >= nextAttackTime)
        {
            isAttacking = true;
            StopAllCoroutines();
            StartCoroutine(AttackCoroutine());

            Unit playerUnit = other.gameObject.GetComponent<Unit>(); // repeatedly attack player
            playerUnit.TakeDamage(unit.damage);
            other.gameObject.GetComponent<HitFlash>().Flash(); // flash player

            StartCoroutine(AttackCooldown());
        }
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
        nextAttackTime = Time.time + attackCooldown;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (status == SkeletonStatus.PlayerAttack && other.gameObject.name == "Player")
        { // stop attack
            isAttacking = false;
            StopCoroutine(AttackCoroutine());
        }
    }

    public IEnumerator AttackCoroutine()
    {
        string animationName;

        switch (orientation)
        {
            case Orientation.UP:
                animationName = "SkeletonAttackUp";
                break;
            case Orientation.DOWN:
                animationName = "SkeletonAttackDown";
                break;
            default:
                animationName = "SkeletonAttackHorizontal";
                break;
        }

        animator.SetBool("walk", false);
        animator.SetTrigger("attack");
        yield return new WaitForSeconds(GameController.GetAnimationLength(animator, animationName)+0.5f);

        animator.ResetTrigger("attack");
        animator.SetBool("walk", true);

    }

    private List<Node> AStarPathfinding(Vector3 start, Vector3 goal)
    {
        Node startNode = mapPath.GetNode(start);
        Node goalNode = mapPath.GetNode(goal);
        if (startNode == null || goalNode == null)
        {
            return null;
        }
        
        float minus = 0.1f;
        float it = 1;
        while (!goalNode.IsWalkable){
            goalNode = mapPath.GetNode(new Vector3(goal.x, goal.y - minus*it, goal.z)); // try to get to walkable range
            it++;
        }

        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FCost < currentNode.FCost || (openList[i].FCost == currentNode.FCost && openList[i].HCost < currentNode.HCost))
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == goalNode)
            {
                return RetracePath(startNode, goalNode);
            }

            foreach (Node neighbor in GetNeighbors(currentNode)) // get neighbors
            {
                if (!neighbor.IsWalkable || closedList.Contains(neighbor))
                {
                    continue;
                }

                float newGCost = currentNode.GCost + GetDistance(currentNode, neighbor);
                if (newGCost < neighbor.GCost || !openList.Contains(neighbor))
                {
                    neighbor.GCost = newGCost;
                    neighbor.HCost = GetDistance(neighbor, goalNode);
                    neighbor.Parent = currentNode;

                    if (!openList.Contains(neighbor)&&!neighbor.Occupied)
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }
        return null;
    }

    private List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        Vector3Int[] directions = {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0)
        };

        foreach (Vector3Int direction in directions)
        {
            Node neighbor = mapPath.GetNode(node.Position + direction);
            if (neighbor != null)
            {
                neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }

    private float GetDistance(Node a, Node b)
    {
        int dstX = Mathf.Abs(a.Position.x - b.Position.x);
        int dstY = Mathf.Abs(a.Position.y - b.Position.y);
        return dstX + dstY;
    }

    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }

    private void CheckIfStuck()
    {
        if (Vector3.Distance(rb.position, lastPosition) < 0.1f)
        {
            stuckTime += Time.deltaTime;
            if (stuckTime >= 5f)
            {
                MoveToRandomNeighbor();
                stuckTime = 0f; 
            }
        }
        else
        {
            stuckTime = 0f;
        }

        lastPosition = rb.position;
    }

    private void MoveToRandomNeighbor()
    {
        Node currentNode = mapPath.GetNode(rb.position);
        List<Node> neighbors = GetNeighbors(currentNode);

        if (neighbors.Count > 0)
        {
            Node randomNeighbor = neighbors[Random.Range(0, neighbors.Count)];
            Vector3 direction = (ground.CellToWorld(randomNeighbor.Position) - (Vector3)rb.position).normalized;
            rb.MovePosition((Vector3)rb.position + direction * moveSpeed * Time.deltaTime);

            UpdateOrientation();
            skeletonPos = rb.position; 
        }
    }

    private void SabotageTorch()
    {
        void Reset(){
            skeletonMoves[SkeletonStatus.TorchSabotage].Remove(this);
            skeletonMoves[SkeletonStatus.PlayerAttack].Add(this);
            currentTorch = 0;

            SkeletonMove skeletonMove = gameObject.AddComponent<SkeletonMove>(); // completely reset component
            skeletonMove.status = SkeletonStatus.PlayerAttack;
            Debug.Log("resetted");
            this.enabled = false; 
        }
        if (torches==null){
            return;
        }
        if (currentTorch<torches.Count){
            Torch torch = torches[currentTorch].GetComponent<Torch>();
            if (torch!=null&&!torch.sabotaged){
                // sabotage torch
                Transform torchTransform = torches[currentTorch];
                Light2D torchLight = torchTransform.GetChild(0).GetComponent<Light2D>();
                StopAllCoroutines();
                StartCoroutine(AttackCoroutine());
                if (torchLight != null)
                {
                    torchLight.intensity = 0.1f; 
                }

                torch.sabotaged = true;
                     
                path = null; // reset path

                currentTorch++;
                // move to next torch     
                if (currentTorch>=torches.Count){
                    Reset();
                }
            }
        }

    }


}
