# 🛗 Elevator Priority Queue — Heap Visualizer

An interactive C# Windows Forms application that visualizes a Min-Heap (Priority Queue) data structure simulating a smart elevator dispatching system. 
This tool bridges the gap between core computer science concepts and practical graphic user interfaces.


## 🚀 Features

- Dynamic Heap Visualization:** Watch the binary tree update its structure in real-time as you manipulate data.
- Min-Heap Logic: Implements strict priority management where lower floor numbers represent the highest priority.
- Smart Operations: "Add Request" : Inserts a passenger request and runs "HeapifyUp".
  - Extract Min O(log n): Resolves the highest priority request (the Root), replaces it, and runs "HeapifyDown".
  - Peek Min` O(1): Inspects the next floor in line without removing it.
- Live Operation Log: Tracks system events, execution timestamps, and heap state mutations on a dedicated side panel.
- Demo Loader: Instantly populates the system with pre-configured datasets for fast testing.

---

## 🛠️ Tech Stack & Concepts Covered

- Language: C# (.NET)
- Framework: Windows Forms (GUI Desktop App)
- Data Structures: Min-Heap, Arrays/Lists, Memory Efficient Structs & Tuples.
- Algorithms: Heapify-Up, Heapify-Down, Priority Queue Scheduling.

---

## 🖥️ How it Works (Under the Hood)

1. The Request: When a passenger inputs their name and destination floor, a lightweight "struct" node is pushed into an underlying array-based List.
2. The Sorting: The "HeapifyUp" algorithm shifts the node to its correct mathematical position based on the formula:
   {Parent Index} = {i - 1}/2
3. The Dispatch: As the elevator moves ("ExtractMin"), the system pulls the Root node (Index = 0) and re-balances the remaining tree structure via:
   $$\text{Left Child} = 2i + 1 \quad\mid\quad \text{Right Child} = 2i + 2

---

## 🔧 Installation & Running

1. Clone this repository:
   ```bash
   git clone [https://github.com/Abdulrahmankh967/Elevator-Heap-Visualizer.git](https://github.com/Abdulrahmankh967/Elevator-Heap-Visualizer.git)
