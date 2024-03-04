# API

- [VuforiaTargetsHandler](#VuforiaTargetsHandler)
- [InstructionsMenu](#InstructionsMenu)
- [ContinuityTest](#ContinuityTest)
- [Change_Scene](#Change_Scene)

## VuforiaTargetsHandler

<i>public class VuforiaTargetsHandler : MonoBehaviour</i>

Reason: to manage the downloading from the server and the instantiation of the Vuforia targets.

### Static properties
| Name               | Type | Value |
|--------------------|------|-------|
| DOWNLOAD_SUCCEEDED | int  | 0     |
| DOWNLOAD_FAILED    | int  | 1     |
| INVALID_INDEX      | int  | 2     |
| EMPTY_TARGETS_LIST | int  | 3     |
| DOWNLOADING        | int  | 4     |

### Properties
| Name                | Access     | Type             | Description                                                            |
|---------------------|------------|------------------|------------------------------------------------------------------------|
| targetsAddressables | Serialized | AssetReference[] | Sorted array of the addressable targets.                               |
| targets             | private    | GameObject[]     | Sorted array of the targets instances.                                 |
| currentTarget       | private    | int              | Index of the currently instantiated target.                            |
| menuCallback        | private    | Action\<int\>      | Callback to inform the InstructionMenu class of the downloading state. |

### Methods

#### public void startTargets\(\)
- Behavior: If the <i>targetsAddressables</i> array has length at least 1, it initializes the <i>targets</i> array and instantiates the first target.
- Parameters: -
- Returns: -

#### public void jumpToTarget\(int id\)
- Behavior: Destroys the currently instantiated target, and instantiates the target of index <i>id</i>.
- Parameters:
  | Name | Type | Mandatory | Description                         |
  |------|------|-----------|-------------------------------------|
  | id   | int  | yes       | Index of the target to instantiate. |
- Returns: -

#### public void nextTarget\(\)
- Behavior: Jumps to the next target w.r.t the currently instantiated one.
- Parameters: -
- Returns: -

#### public void previousTarget\(\)
- Behavior: Jumps to the previous target w.r.t the currently instantiated one.
- Parameters: -
- Returns: -


## InstructionsMenu

<i>public class InstructionsMenu : MonoBehaviour</i>

Reason: to manage the steps menu, i.e. to provide handlers to the buttons and modify the shown message.

### Static properties
| Name                        | Type   | Value                                                  |
|-----------------------------|--------|--------------------------------------------------------|
| CURRENT_INSTRUCTION_STRING  | string | "Current instruction: "                                |
| EMPTY_TARGETS_LIST_STRING   | string | "The instructions list is empty!"                      |
| SERVER_ERROR_STRING         | string | "Download from server failed."                         |
| INVALID_INDEX_STRING        | string | "Invalid value. Please choose a number between 1 and " |
| DOWNLOADING_STRING          | string | "Downloading target from server..."                    |
| WAIT_FINISH_DOWNLOAD_STRING | string | "Wait for the previous download to finish!"            |

### Properties
| Name                     | Access     | Type                  | Description                                                    |
|--------------------------|------------|-----------------------|----------------------------------------------------------------|
| vuforiaTargetsHandlerObj | Serialized | GameObject            | Object containing a VuforiaTargetsHandler component.           |
| vuforiaTargetsHandler    | private    | VuforiaTargetsHandler | Component of the vuforiaTargetsHandlerObj object.              |
| textToShowObj            | Serialized | GameObject            | Object containing a component to show a message in the panel.  |
| textToShow               | private    | TextMeshProUGUI       | Editable text component of the textToShowObj object.           |
| downloading              | private    | bool                  | Indicates whether a target is being downloaded in this moment. |

### Methods

#### public void nextInstruction\(\)
- Behavior: Asks the vuforiaTargetsHandler to jump to the next target. It's the handler of the "next" button.
- Parameters: -
- Returns: -

#### public void previousInstruction\(\)
- Behavior: Asks the vuforiaTargetsHandler to jump to the previous target. It's the handler of the "previous" button.
- Parameters: -
- Returns: -

#### public void skipToInstructionNumber\(int instruction\)
- Behavior: Asks the <i>vuforiaTargetsHandler</i> to jump to the target of index <i>instruction</i> - 1.
- Parameters:
  | Name | Type | Mandatory | Description                         |
  |------|------|-----------|-------------------------------------|
  | instruction   | int  | yes       | Index \(+1\) of the target to jump to. |
- Returns: -


## ContinuityTest

<i>public class ContinuityTest : MonoBehaviour</i>

Reason: to request data from the multimeter server and show them in a panel.

### Properties
| Name             | Access     | Type            | Description                                                             |
|------------------|------------|-----------------|-------------------------------------------------------------------------|
| textObj          | Serialized | GameObject      | Object containing a component to show the multimeter data in the panel. |
| textComponent    | private    | textComponent   | Text component of the textObj object.                                   |
| messageObj       | Serialized | GameObject      | Object containing a component to show a message in the panel.           |
| messageComponent | private    | TextMeshProUGUI | Text component of the messageObj object.                                |
| URL              | private    | string          | URL of the multimeter server.                                           |

Change_Scene

#### public void startRequestingValues\(\)
- Behavior: Starts a coroutine in which the <i>getValuesCoroutine</i> function is executed, with input parameter <i>URL</i>.
- Parameters: -
- Returns: -

#### IEnumerator getValuesCoroutine\(string url\)
- Behavior: In a loop, it requests data from the server at address <i>url</i>, it uses these data to update the panel, and then waits 0.3 seconds before requesting new data. 
- Parameters:
  | Name | Type | Mandatory | Description                         |
  |------|------|-----------|-------------------------------------|
  | url   | string  | yes       | URL of the server. |
- Returns:
  | Type | Description                         |
  |------|-------------------------------------|
  | IEnumerator  | A function must return a IEnumerator object to be executed in a coroutine. |


## Change_Scene

<i>public class Change_Scene : MonoBehaviour</i>

Reason: to provide a handler to the buttons that need to change the scene.

### Methods

#### public void MoveToScene\(int sceneID\)
- Behavior: Loads the scene of index <i>sceneID</i>.
- Parameters:
  | Name | Type | Mandatory | Description                         |
  |------|------|-----------|-------------------------------------|
  | sceneID   | int  | yes       | Index of the scene to load. |
- Returns: -