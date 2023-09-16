using App.Scripts.Scenes.SceneChess.Features.ChessField.GridMatrix;
using App.Scripts.Scenes.SceneChess.Features.ChessField.Types;
using System.Collections.Generic;
using UnityEngine;

namespace App.Scripts.Scenes.SceneChess.Features.GridNavigation.Navigator
{
    //этот код не доконца доделан поэтому он может приводить к ошибкам и не реализует полностью необходимые требования
    public class ChessGridNavigator : IChessGridNavigator
    {
        public List<Vector2Int> FindPath(ChessUnitType unit, Vector2Int from, Vector2Int to, ChessGrid grid)
        {
            var listPositionWithoutEstimate = new List<Vector2Int>();
            var listPositionEstimated = new List<Vector2Int>();
            var cameFromBetweenSteps = new Dictionary<Vector2Int, Vector2Int>();
            var distanceBetweenStartToCurrent = new Dictionary<Vector2Int, int>();
            var heuristicDistanse = new Dictionary<Vector2Int, int>();

            listPositionWithoutEstimate.Add(from);
            distanceBetweenStartToCurrent[from] = 0;
            heuristicDistanse[from] = HeuristicCostEstimate(from, to);

            while (listPositionWithoutEstimate.Count > 0)
            {
                var current = GetLowestEristicDistanse(listPositionWithoutEstimate, heuristicDistanse);

                if (current == to)
                {
                    var path = ReconstructPath(cameFromBetweenSteps, current);
                    path.Reverse();
                    return path;
                }

                listPositionWithoutEstimate.Remove(current);
                listPositionEstimated.Add(current);

                var neighbors = GetPossibleMovesForUnit(unit, current, grid);

                foreach (var neighbor in neighbors)
                {
                    if (listPositionEstimated.Contains(neighbor))
                        continue;

                    var tentativeDistanceBetvinStartToCurrent = distanceBetweenStartToCurrent[current] + 1;

                    if (!listPositionWithoutEstimate.Contains(neighbor))
                        listPositionWithoutEstimate.Add(neighbor);

                    cameFromBetweenSteps[neighbor] = current;
                    distanceBetweenStartToCurrent[neighbor] = tentativeDistanceBetvinStartToCurrent;
                    heuristicDistanse[neighbor] = distanceBetweenStartToCurrent[neighbor] + HeuristicCostEstimate(neighbor, to);
                }
            }

            return null;
        }

        private int HeuristicCostEstimate(Vector2Int from, Vector2Int to)
        {
            return Mathf.Abs(to.x - from.x) + Mathf.Abs(to.y - from.y);
        }

        private Vector2Int GetLowestEristicDistanse(List<Vector2Int> listPositionWithoutEstimation, Dictionary<Vector2Int, int> eristicDistanse)
        {
            var lowest = int.MaxValue;
            Vector2Int lowestNode = default;

            foreach (var node in listPositionWithoutEstimation)
            {
                if (eristicDistanse.ContainsKey(node) && eristicDistanse[node] < lowest)
                {
                    lowest = eristicDistanse[node];
                    lowestNode = node;
                }
            }

            return lowestNode;
        }

        private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
        {
            var path = new List<Vector2Int>();

            while (cameFrom.ContainsKey(current))
            {
                path.Add(current);
                current = cameFrom[current];
            }

            return path;
        }

        private List<Vector2Int> GetPossibleMovesForUnit(ChessUnitType unit, Vector2Int position, ChessGrid grid)
        {
            var possibleMoves = new List<Vector2Int>();
            var color = DetermineColor(position, grid);

            switch (unit)
            {
                case ChessUnitType.Pon:
                    var forwardStep = (color == ChessUnitColor.White) ? 1 : -1;

                    var forward = new Vector2Int(position.x, position.y + forwardStep);
                    if (IsValidMove(forward.x, forward.y, grid))
                    {
                        possibleMoves.Add(forward);

                        if ((color == ChessUnitColor.White && position.y == 1) || (color == ChessUnitColor.Black && position.y == 6))
                        {
                            var doubleForward = new Vector2Int(position.x, position.y + 2 * forwardStep);
                            if (IsValidMove(doubleForward.x, doubleForward.y, grid))
                            {
                                possibleMoves.Add(doubleForward);
                            }
                        }
                    }

                    var attackRight = new Vector2Int(position.x + 1, position.y + forwardStep);
                    var attackLeft = new Vector2Int(position.x - 1, position.y + forwardStep);

                    if (IsValidMove(attackRight.x, attackRight.y, grid)) possibleMoves.Add(attackRight);
                    if (IsValidMove(attackLeft.x, attackLeft.y, grid)) possibleMoves.Add(attackLeft);

                    break;

                case ChessUnitType.Bishop:
                    for (int i = 1; i < grid.Size.x; i++)
                    {
                        var upRight = new Vector2Int(position.x + i, position.y + i);
                        var upLeft = new Vector2Int(position.x - i, position.y + i);
                        var downRight = new Vector2Int(position.x + i, position.y - i);
                        var downLeft = new Vector2Int(position.x - i, position.y - i);

                        if (IsValidMove(upRight.x, upRight.y, grid))
                        {
                            if (grid.Get(upRight) == null) possibleMoves.Add(upRight);
                            else break;
                        }

                        if (IsValidMove(upLeft.x, upLeft.y, grid))
                        {
                            if (grid.Get(upLeft) == null) possibleMoves.Add(upLeft);
                            else break;
                        }

                        if (IsValidMove(downRight.x, downRight.y, grid))
                        {
                            if (grid.Get(downRight) == null) possibleMoves.Add(downRight);
                            else break;
                        }

                        if (IsValidMove(downLeft.x, downLeft.y, grid))
                        {
                            if (grid.Get(downLeft) == null) possibleMoves.Add(downLeft);
                            else break;
                        }
                    }

                    break;

                case ChessUnitType.Rook:
                    for (int i = 1; i < grid.Size.x; i++)
                    {
                        var up = new Vector2Int(position.x, position.y + i);
                        var down = new Vector2Int(position.x, position.y - i);
                        var left = new Vector2Int(position.x - i, position.y);
                        var right = new Vector2Int(position.x + i, position.y);

                        if (IsValidMove(up.x, up.y, grid) && !HasPiecesOnPath(position.x, position.y, up.x, up.y, grid))
                            possibleMoves.Add(up);
                        if (IsValidMove(down.x, down.y, grid) && !HasPiecesOnPath(position.x, position.y, down.x, down.y, grid))
                            possibleMoves.Add(down);
                        if (IsValidMove(left.x, left.y, grid) && !HasPiecesOnPath(position.x, position.y, left.x, left.y, grid))
                            possibleMoves.Add(left);
                        if (IsValidMove(right.x, right.y, grid) && !HasPiecesOnPath(position.x, position.y, right.x, right.y, grid))
                            possibleMoves.Add(right);
                    }

                    break;

                case ChessUnitType.Queen:
                    for (int i = 1; i < grid.Size.x; i++)
                    {
                        var up = new Vector2Int(position.x, position.y + i);
                        var down = new Vector2Int(position.x, position.y - i);
                        var left = new Vector2Int(position.x - i, position.y);
                        var right = new Vector2Int(position.x + i, position.y);

                        if (IsValidMove(up.x, up.y, grid))
                            possibleMoves.Add(up);
                        if (IsValidMove(down.x, down.y, grid))
                            possibleMoves.Add(down);
                        if (IsValidMove(left.x, left.y, grid))
                            possibleMoves.Add(left);
                        if (IsValidMove(right.x, right.y, grid))
                            possibleMoves.Add(right);
                    }

                    for (int i = 1; i < grid.Size.x; i++)
                    {
                        var upLeft = new Vector2Int(position.x - i, position.y + i);
                        var upRight = new Vector2Int(position.x + i, position.y + i);
                        var downLeft = new Vector2Int(position.x - i, position.y - i);
                        var downRight = new Vector2Int(position.x + i, position.y - i);

                        if (IsValidMove(upLeft.x, upLeft.y, grid))
                            possibleMoves.Add(upLeft);
                        if (IsValidMove(upRight.x, upRight.y, grid))
                            possibleMoves.Add(upRight);
                        if (IsValidMove(downLeft.x, downLeft.y, grid))
                            possibleMoves.Add(downLeft);
                        if (IsValidMove(downRight.x, downRight.y, grid))
                            possibleMoves.Add(downRight);
                    }

                    break;

                case ChessUnitType.King:
                    var possibleKingMoves = new List<Vector2Int>
                    {
                        new Vector2Int(position.x + 1, position.y),
                        new Vector2Int(position.x + 1, position.y + 1),
                        new Vector2Int(position.x, position.y + 1),
                        new Vector2Int(position.x - 1, position.y + 1),
                        new Vector2Int(position.x - 1, position.y),
                        new Vector2Int(position.x - 1, position.y - 1),
                        new Vector2Int(position.x, position.y - 1),
                        new Vector2Int(position.x + 1, position.y - 1)
                    };

                    foreach (var move in possibleKingMoves)
                    {
                        if (IsValidMove(move.x, move.y, grid))
                            possibleMoves.Add(move);
                    }

                    break;

                case ChessUnitType.Knight:
                    var possibleKnightMoves = new List<Vector2Int>
                    {
                        new Vector2Int(position.x + 1, position.y + 2),
                        new Vector2Int(position.x + 2, position.y + 1),
                        new Vector2Int(position.x + 2, position.y - 1),
                        new Vector2Int(position.x + 1, position.y - 2),
                        new Vector2Int(position.x - 1, position.y - 2),
                        new Vector2Int(position.x - 2, position.y - 1),
                        new Vector2Int(position.x - 2, position.y + 1),
                        new Vector2Int(position.x - 1, position.y + 2)
                    };

                    foreach (var move in possibleKnightMoves)
                    {
                        if (IsValidMove(move.x, move.y, grid))
                            possibleMoves.Add(move);
                    }

                    break;
                default:
                    break;
            }

            return possibleMoves;
        }

        public ChessUnitColor DetermineColor(Vector2Int position, ChessGrid grid)
        {
            int halfHeight = grid.Size.y / 2;
            return (position.y < halfHeight) ? ChessUnitColor.White : ChessUnitColor.Black;
        }

        private bool IsValidMove(int x, int y, ChessGrid grid)
        {
            if (x < 0 || x >= grid.Size.x || y < 0 || y >= grid.Size.y) return false;

            var targetUnit = grid.Get(y, x);
            if (targetUnit != null) return false;

            return true;
        }

        private bool HasPiecesOnPath(int startX, int startY, int targetX, int targetY, ChessGrid grid)
        {
            int stepX = (targetX > startX) ? 1 : (targetX < startX) ? -1 : 0;
            int stepY = (targetY > startY) ? 1 : (targetY < startY) ? -1 : 0;

            int currentX = startX + stepX;
            int currentY = startY + stepY;

            while (currentX != targetX || currentY != targetY)
            {
                var unit = grid.Get(currentX, currentY);
                if (unit != null) return true;

                currentX += stepX;
                currentY += stepY;
            }

            return false;
        }
    }
}