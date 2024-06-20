using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonMove : MonoBehaviour
{
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

    void Start()
    {
        orientation = Orientation.RIGHT;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        unit = GetComponent<Unit>();
        rb = GetComponent<Rigidbody2D>();

        mapPath = MapPath.instance;

        player = GameObject.FindGameObjectWithTag("Player");

        skeletonPos = rb.position; // Use rb.position instead of transform.position
    }

    void Update()
    {
        moveSpeed = MapManager.instance.GetWalkingSpeed(rb.position) * 1.5f;
        Vector3 currentPlayerPos = player.GetComponent<Rigidbody2D>().position;

        if (playerPos == null || Vector3.Distance(currentPlayerPos, playerPos) > 0.01f)
        {
            playerPos = currentPlayerPos;

            // re-navigate
            FindPathToPlayer();
        }

        MoveAlongPath();
    }

    private void FindPathToPlayer()
    {
        path = AStarPathfinding(rb.position, playerPos);

        if (path != null && path.Count > 0)
        {
            currentPathIndex = 0;
        }
    }

    private void MoveAlongPath()
    {
        if (path == null || currentPathIndex >= path.Count)
            return;

        Vector3 targetPosition;

        // reached goal then move toward nearer to the player
        if (currentPathIndex == path.Count - 1)
        {
            targetPosition = playerPos;

            if (Vector3.Distance(rb.position, playerPos) <= 0.5f)
            {
                return;
            }
            else{
                rb.MovePosition(Vector3.MoveTowards(rb.position, playerPos, 0.5f));
            }
        }
        else
        {
            targetPosition = path[currentPathIndex].Position + new Vector3(0.5f, 0.5f, 0);
            if (Vector3.Distance(rb.position, targetPosition) < 0.1f)
            {
                currentPathIndex++;
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
        Debug.Log(orientation);
    }

    private void SetOrientation(Orientation newOrientation)
    {
        if (orientation != newOrientation)
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
    public float attackCooldown = 1f; 

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isAttacking && other.gameObject.name == "Player" && Time.time >= nextAttackTime)
        {
            isAttacking = true;

            StartCoroutine(AttackCoroutine());
            StartCoroutine(AttackCooldown());

            Unit playerUnit = other.gameObject.GetComponent<Unit>();
            playerUnit.TakeDamage(unit.damage);
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
        if (other.gameObject.name == "Player")
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

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName(animationName)) // make sure not playing the current animation
        {
            animator.SetBool("walk", false);
            animator.SetTrigger("attack");
            yield return new WaitForSeconds(GameController.GetAnimationLength(animator, animationName)+0.5f);

            animator.ResetTrigger("attack");
            animator.SetBool("walk", true);
        }
    }

    private List<Node> AStarPathfinding(Vector3 start, Vector3 goal)
    {
        Node startNode = mapPath.GetNode(start);
        Node goalNode = mapPath.GetNode(goal);
        if (startNode == null || goalNode == null || !startNode.IsWalkable || !goalNode.IsWalkable)
        {
            return null;
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

            foreach (Node neighbor in GetNeighbors(currentNode))
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

                    if (!openList.Contains(neighbor))
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
}
