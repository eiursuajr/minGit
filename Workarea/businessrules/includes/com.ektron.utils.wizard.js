var WizardUtil =
{
    showNextStep: function()
    {
        var currentStep = WizardUtil.getCurrentStep();
        var nextStep = currentStep + 1;
        WizardUtil.setStep(nextStep);
        WizardUtil.showStep(nextStep);
    },

    showPreviousStep: function()
    {
        var currentStep = WizardUtil.getCurrentStep();
        var prevStep = currentStep - 1;
        WizardUtil.setStep(prevStep);
        WizardUtil.showStep(prevStep);
    },

    showStep: function( step )
    {
        // remember the number of our current step
        WizardUtil.setStep(step);
        
        var wizardContainer = document.getElementById( "wizard" );

        var stepContainer = WizardUtil.getStep(step);
        if( stepContainer != null ) {
            // hide all steps
            for( var i = 0; i < wizardContainer.childNodes.length; i++ ) {
                if (document.all)
                {
                    var child = wizardContainer.childNodes[i];
                    if (child.style != undefined) {
                        child.style["display"] = "none";
                    }
                }
                else
                {
                    var steper = document.getElementById( "wizardStep" + i );
                    if (steper)
                    {
                        //alert(steper.style.display);
                        steper.style.display = "none";   
                    }
                }
            }
            // show current step
            stepContainer.style["display"] = "block";
        }
    },
    
    getStep: function( step )
    {
        return document.getElementById( "wizardStep" + step );
    },

    setStep: function( step )
    {
        var currentStepElement = document.getElementById( "wizardCurrentStep" );
        if( currentStepElement == null ) {
            currentStepElement = document.createElement( "input" );
            currentStepElement.type = "hidden";
            currentStepElement.name = "wizardCurrentStep";
            currentStepElement.id = "wizardCurrentStep";
            document.body.appendChild(currentStepElement);
        }

        // make sure it is a valid step, e.g. that we have a step, before setting it
        if( WizardUtil.getStep( step ) != null ) {
            currentStepElement.value = step;
        }
    },

    getCurrentStep: function()
    {
        var currentStepElement = document.getElementById( "wizardCurrentStep" );
        if( currentStepElement == null ) {
            // if we don't have it yet, create it and initialize to 1
            WizardUtil.setStep(1);
            currentStepElement = document.getElementById( "wizardCurrentStep" );
        }

        return parseInt( currentStepElement.value, 10 );
    }
}