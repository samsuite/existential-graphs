function AddUIReactors(proof) {
	proof.addReactor(Proof.EVENTS.CHANGE_MODE, ChangeMode);
	proof.addReactor(Proof.EVENTS.ADD_NODE, AddNode);
	proof.addReactor(Proof.EVENTS.SELECT_NODE, ChangeNode);
	proof.addReactor(Proof.EVENTS.NEXT_NODE, ChangeNode);
	proof.addReactor(Proof.EVENTS.PREVIOUS_NODE, ChangeNode);
}

function ChangeMode(proof) {
	var mode = proof.currentMode;
	var mode_name = "";
	var warning_color = "";
	if (mode === Proof.LOGIC_MODES.PREMISE_MODE) {
		mode_name = "Premise Mode";
		warning_color = "label-success";
	}
	if (mode === Proof.LOGIC_MODES.PROOF_MODE) {
		mode_name = "Proof Mode";
		warning_color = "label-info";
	}
	if (mode === Proof.LOGIC_MODES.INSERTION_MODE) {
		mode_name = "Insertion Mode";
		warning_color = "label-warning";
	}
	if (mode === Proof.LOGIC_MODES.GOAL_MODE) {
		mode_name = "Goal Mode";
		warning_color = "label-danger";
	}
	$('#ModeLinkContainer').html('<div id="ModeLink"class="col-sm-12 '+ warning_color +'">'+mode_name+'</div>');
	$('#ModeLink').click(function() {
		proof.endCurrentProofMode();
	});
}

function AddNode(proof) {
	//if(proof.currentMode !== Proof.LOGIC_MODES.GOAL_MODE
	//&& proof.currentMode !== Proof.LOGIC_MODES.PREMISE_MODE) {
	//	proof.automated_check(proof.current);
	//}
	branches.draw.call(Timeline, proof);
	TheProof.UIReset();
	minimap.redraw();
}

function ChangeNode(proof) {
	branches.draw.call(Timeline, proof);
	TheProof.TickRefresh();
	TheProof.UIReset();
	minimap.redraw();
}
