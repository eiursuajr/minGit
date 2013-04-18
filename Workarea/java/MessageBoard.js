
  Ektron.ready(
    function()
    {
        if ("undefined" == typeof Ektron.MessageBoard)
        {
            Ektron.MessageBoard = 
            {
                // Properties
                currentMessageId: 0,
                replyAlertresource: "",
                
                // Methods
                CenterModal: function(modalSelector)
                {
                    $ektron(modalSelector).css("margin-top", -1 * Math.round($ektron(modalSelector).outerHeight()/2));
                },
                CreateModal: function(id,commentResource,maxResource,cancelResource,replyResource)
                {
                    var modalId = "MessageBoardModal" + id;
                    var modalSelector = "#" + modalId;
                    var thisMessageBoard = $ektron(modalSelector);
                    
                    if (thisMessageBoard.length == 0)
                    {
                        var pageBody = $ektron("body");
                        var modalHtml = '<div class="ektronWindow ektronModalStandard ektronMessageBoardModal" id="' + modalId + '">';
                        modalHtml += '  <div class="ektronModalHeader">';
                        modalHtml += '      <h3>';
                        modalHtml += '          <span class="headerText">'+ replyResource +'</span>';
                        modalHtml += '          <a href="#Close" class="ektronModalClose"  title="Close"><span style="visibility:hidden;">Close</span></a>';
                        modalHtml += '      </h3>';
                        modalHtml += '  </div>';
                        modalHtml += '  <div class="ektronModalBody">';
                        modalHtml += '      <div class="ektronModalHeight-10">';
                        modalHtml += '      <p class="messages"></p>';
                        modalHtml += '      <div class="ekMsgBoardReplyPanelComment"><h4>'+commentResource+'</h4><blockquote>Comment</blockquote></div>';
                        modalHtml += '      </div>';
                        modalHtml += '      <br />';
                        modalHtml += '      <h4>'+ replyResource +'</h4>';
                        modalHtml += '      <p class="ekMsgBoardReplyTextAreaWrapper">';
                        modalHtml += '          <textarea id="ReplyContributionText'+ id + '"  name="ReplyContributionText'+ id +'"  class="ReplyContributionText" rows="5" maxlength="2000"></textarea>';
                        modalHtml += '      </p>';
                        modalHtml += '      <span class="ekMessageBoardCaption">'+ maxResource +'</span>';
                        modalHtml += '      <div class="ektronModalButtonWrapper clearfix">';
                        modalHtml += '          <ul class="buttonWrapper clearfix">';
                        modalHtml += '              <li><a class="button buttonRight redHover cancelButton" onclick="$ektron(\'' + modalSelector + '\').modalHide();return false;">'+ cancelResource +'</a>';
                        modalHtml += '                  <a class="button buttonRight greenHover ekMessageBoardReplyButton" onclick="Ektron.MessageBoard.AddReply(\'' + id + '\');return false;">'+ replyResource +'</a></li>';
                        modalHtml += '          </ul>';
                        modalHtml += '      </div>';
                        modalHtml += '  </div>';
                        modalHtml += '</div>';
                        
                        // prepend the modal HTML to the body
                        pageBody.prepend(modalHtml);
                        theModal = $ektron(modalSelector);

                        // make the modal
                        theModal.modal({
                            modal: true,
                            toTop: true,
                            onShow: function(hash) {
                                $ektron(modalSelector).find("textarea").val("");
                                $ektron(modalSelector).find(".messages").empty();
                                Ektron.MessageBoard.CenterModal(modalSelector);
			                    hash.o.fadeTo("fast", 0.5, function() {
				                    hash.w.fadeIn("fast");
			                    });
                            },
                            onHide: function(hash) {
                                hash.w.fadeOut("fast");
			                    hash.o.fadeOut("fast", function() {
				                    if (hash.o)
				                    {
					                    hash.o.remove();
			                        }
			                    });
                            }
                        });
                      
                    }
                },
                Init: function(id, commentResource, maxCharacterResource, cancelResource, replyResource)
                {
                    Ektron.MessageBoard.CreateModal(id,commentResource, maxCharacterResource, cancelResource,replyResource);                    
                },
                SetReply: function(modalSelector, messageId, comment)
                {
                    
                    var modal = $ektron("#MessageBoardModal"+ modalSelector);
                    var blockquote = modal.find("blockquote");
                    blockquote.html("<span class='bqStart'>&#8220;</span>" + comment + "<span class='bqEnd'>&#8221;</span>");
                    Ektron.MessageBoard.currentMessageId = messageId;
                    modal.modalShow();
                    return false;
                },
                ShowApprove: function(id,mode)
                {
                    //var searchDivId = "EkMicroMessageSearch" + id
                    //var searchDiv = $ektron("#" + searchDivId);
                    var inputDiv = $ektron("#EkMessageBoardInput" + id);
                    //var listDiv = $ektron("#EkMicroMessageDisplay"+ id);
                    
                    if(mode == "approve")
                    {
                        
                        inputDiv.attr("style","display:none");
                        $ektron("#dvGeneralTab"+id).removeClass('active');
                        $ektron("#dvApproveTab"+id).addClass('active');
                        
                        
                    } 
                    else
                    {
                       
                        inputDiv.attr("style","display:block");
                        $ektron("#dvApproveTab"+id).removeClass('active');
                        $ektron("#dvGeneralTab"+id).addClass('active');
                        
                         
                    }
                    return false;
                },
                AddReply: function(uniqueId)
                {
                    
                    var replyText = '';    
                    var modal = $ektron("#MessageBoardModal"+ uniqueId);
                    var replyTextArea = $ektron('#ReplyContributionText'+ uniqueId ); 
                                       
                    if (replyTextArea.length > 0)
                     {
                       replyText = replyTextArea.val();
                     }
                    
                    if (replyText == '')
                    {
                      
                      var message = Ektron.MessageBoard.replyAlertresource;
                      messageContainer= modal.find(".messages");
                      messageContainer.empty();
                      messageContainer.html('<span class="error">' + message + "</span>").fadeIn("slow");
                       return;
                    }
                    replyText=escape(replyText);
                    var functionName ='_PostReplyMessages' + uniqueId;
                    (window[functionName])(IAjax.getArguements(),'control=' + uniqueId + '&ReplyContributionText' + uniqueId + '=' + replyText + '&messageId='+ Ektron.MessageBoard.currentMessageId + '&__ecmmsgaction' + uniqueId + '=replymessage');
                    modal.modalHide();
                }
            }
            
        }
    }
);








